using System.Drawing;

using Starlight.Device.Display.Font;

//
// https://github.com/WildernessLabs/Netduino.Foundation/blob/67d190b2ac46165d05e1268c3f9d75e7bf651649/Source/Peripheral_Libs/Displays.SSD1306/Driver/SSD1306.cs
//
namespace Starlight.Device.I2c
{
    public class DisplaySSD1306 : Base.I2cDevice
    {
        /// <summary>
        /// Height of the display
        /// </summary>
        public byte Height { get; private set; }

        /// <summary>
        /// Width of the display
        /// </summary>
        public byte Width { get; private set; }

        /// <summary>
        /// Type of the display
        /// </summary>
        public DisplayType Type { get; private set; }

        /// <summary>
        /// Current Font to use when displaying text
        /// </summary>
        public FontBase CurrentFont { get; set; } = new Display.Font.Font4x8();

        /// <summary>
        /// Get / Put the display to sleep (turns the display off).
        /// </summary>
        public bool IsAsleep
        {
            get => _isAsleep;
            set
            {
                if(_isAsleep == value) { return; }

                _isAsleep = value;
                this.SendCommand((byte)(_isAsleep ? 0xae : 0xaf));
            }
        }
        private bool _isAsleep = false;

        /// <summary>
        /// Get / Set if the display is inverted
        /// </summary>
        public bool IsInverted
        {
            get => _isInverted;
            set
            {
                if(_isInverted == value) { return; }

                _isInverted = value;
                this.SendCommand((byte)(value ? 0xa7 : 0xa6));
            }
        }
        private bool _isInverted = false;

        /// <summary>
        /// Get / Set the contrast of the display.
        /// </summary>
        public byte Contrast
        {
            get => _contrast;
            set
            {
                _contrast = value;
                this.SendCommands(new byte[] { 0x81, _contrast });
            }
        }
        private byte _contrast;


        /// <summary>
        /// Buffer holding the pixels in the display.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// Sequence of command bytes that must be sent to the display before the Show method can send the data buffer.
        /// </summary>
        private byte[] _showPreamble;

        /// <summary>
        /// Current text cursor position for text writing
        /// </summary>
        public Point CursorPosition { get; set; } = new Point(0, 0);

        public DisplaySSD1306(string name, int id = 0x3c, int busId = 1, DisplayType displayType = DisplayType.OLED128x64) : base(name, id, busId)
        {
            this.Type = displayType;
        }

        public override void Init()
        {

            base.Init();

            switch (this.Type)
            {
                case DisplayType.OLED128x64:
                case DisplayType.OLED64x48:
                    {
                        this.Width = 128;
                        this.Height = 64;
                        this.SendCommands(_oled128x64SetupSequence);
                        break;
                    }
                case DisplayType.OLED128x32:
                    {
                        this.Width = 128;
                        this.Height = 32;
                        this.SendCommands(_oled128x32SetupSequence);
                        break;
                    }
                case DisplayType.OLED96x16:
                    {
                        this.Width = 96;
                        this.Height = 16;
                        this.SendCommands(_oled96x16SetupSequence);
                        break;
                    }
            }

            _buffer = new byte[this.Width * this.Height / 8];
            _showPreamble = new byte[] { 0x21, 0x00, (byte)(this.Width - 1), 0x22, 0x00, (byte)(this.Height / 8 - 1) };

            //  Finally, put the display into a known state.
            this.IsInverted = false;
            this.IsAsleep = false;
            _contrast = 0xff;
            this.StopScrolling();
        }


        //public static DisplaySSD1306 Create(string name, int id = 0x3c, int busId = 1, DisplayType displayType = DisplayType.OLED128x64)
        //{
        //    try
        //    {
        //        var device = new DisplaySSD1306(name, id, busId, displayType);
        //        device.Init();

        //        return device;
        //    }
        //    catch (Exception ex)
        //    {
        //        var originalColor = Console.ForegroundColor;
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("===================================================================");
        //        Console.WriteLine(" I2C OLED DEVICE ERROR");
        //        Console.WriteLine("===================================================================");
        //        Console.WriteLine(ex.Message);
        //        Console.ForegroundColor = originalColor;

        //        return null;
        //    }
        //}

        /// <summary>
        ///     Send a command to the display.
        /// </summary>
        /// <param name="command">Command byte to send to the display.</param>
        private void SendCommand(byte command)
        {
            this.Device.Write(new byte[] { 0x00, command });
        }

        /// <summary>
        /// Send a sequence of commands to the display.
        /// </summary>
        /// <param name="commands">List of commands to send.</param>
        private void SendCommands(byte[] commands)
        {
            var data = new byte[commands.Length + 1];
            data[0] = 0x00;
            Array.Copy(commands, 0, data, 1, commands.Length);

            this.Device.Write(data);
        }

        /// <summary>
        /// Send the internal pixel buffer to display.
        /// </summary>
        public void Show()
        {
            this.SendCommands(_showPreamble);

            //  Send the buffer page by page.
            const int PAGE_SIZE = 16;
            var data = new byte[PAGE_SIZE + 1];
            data[0] = 0x40;

            for (ushort index = 0; index < _buffer.Length; index += PAGE_SIZE)
            {
                Array.Copy(_buffer, index, data, 1, PAGE_SIZE);
                this.SendCommand(0x40);
                this.Device.Write(data);
            }
        }

        /// <summary>
        /// Clear the display buffer.
        /// </summary>
        /// <param name="updateDisplay">Immediately update the display when true.</param>
        public void Clear(bool updateDisplay = false)
        {
            Array.Clear(_buffer, 0, _buffer.Length);

            if (updateDisplay) { this.Show(); }
        }

        /// <summary>
        ///     Coordinates start with index 0
        /// </summary>
        /// <param name="x">Abscissa of the pixel to the set / reset.</param>
        /// <param name="y">Ordinate of the pixel to the set / reset.</param>
        /// <param name="isOn">True = turn on pixel, false = turn off pixel</param>
        public void DrawPixel(int x, int y, bool isOn)
        {
            if ((x >= this.Width) || (y >= this.Height)) { return; }

            if (this.Type == DisplayType.OLED64x48)
            {
                this.DrawPixel64x48(x, y, isOn);
                return;
            }

            var index = (y / 8 * this.Width) + x;

            if (isOn)
            {
                _buffer[index] = (byte)(_buffer[index] | (byte)(1 << (y % 8)));
            }
            else
            {
                _buffer[index] = (byte)(_buffer[index] & ~(byte)(1 << (y % 8)));
            }
        }

        private void DrawPixel64x48(int x, int y, bool isOn)
        {
            if ((x >= 64) || (y >= 48)) { return; }

            //offsets for landscape
            x += 32;
            y += 16;

            var index = (y / 8 * this.Width) + x;

            if (isOn)
            {
                _buffer[index] = (byte)(_buffer[index] | (byte)(1 << (y % 8)));
            }
            else
            {
                _buffer[index] = (byte)(_buffer[index] & ~(byte)(1 << (y % 8)));
            }

            this.CursorPosition = new Point(x, y);
        }

        public void DrawHLine(int x, int y, int width, bool isOn)
        {
            for (int i = 0; i < width; i++)
            {
                this.DrawPixel((x + i), y, isOn);
            }
        }

        public void DrawVLine(int x, int y, int height, bool isOn)
        {
            for (int i = 0; i < height; i++)
            {
                this.DrawPixel(x, (y + i), isOn);
            }
        }

        public void DrawFilledRectangle(int x, int y, int width, int height, bool isOn)
        {
            for (byte i = 0; i < height; i++)
            {
                this.DrawHLine(x, (y + i), width, isOn);
            }
        }

        public void DrawRectangle(int x, int y, int width, int height, bool isOn)
        {
            this.DrawHLine(x, y, width, isOn);    //Top
            this.DrawHLine(x, y + height - 1, width, isOn);    //Bottom

            this.DrawVLine(x, y, height, isOn); //Left
            this.DrawVLine(x + width - 1, y, height, isOn); //Right
        }

        public void DrawRoundRectangle(int x, int y, int width, int height, int radius, bool isOn)
        {
            // draw the non-curved line segments
            this.DrawHLine(x + radius, y, width - 2 * radius, isOn); // Top
            this.DrawHLine(x + radius, y + height - 1, width - 2 * radius, isOn); // Bottom

            this.DrawVLine(x, y + radius, height - 2 * radius, isOn); // Left
            this.DrawVLine(x + width - 1, y + radius, height - 2 * radius, isOn); // Right

            // draw four corners
            this.DrawCornerHelper(x + radius, y + radius, radius, CornerName.NW, isOn);                             //1
            this.DrawCornerHelper(x + width - radius - 1, y + radius, radius, CornerName.SE, isOn);                 //2
            this.DrawCornerHelper(x + width - radius - 1, y + height - radius - 1, radius, CornerName.NE, isOn);    //4
            this.DrawCornerHelper(x + radius, y + height - radius - 1, radius, CornerName.SW, isOn);                //8
        }

        private void DrawCornerHelper(int x0, int y0, int r, CornerName cornername, bool isOn)
        {
            int f = 1 - r;
            int ddF_x = 1;
            int ddF_y = -2 * r;
            int x = 0;
            int y = r;

            while (x < y)
            {
                if (f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x;

                if (cornername == CornerName.NE)    //0x4
                {
                    this.DrawPixel(x0 + x, y0 + y, isOn);
                    this.DrawPixel(x0 + y, y0 + x, isOn);
                }
                if (cornername == CornerName.SE)    //0x2
                {
                    this.DrawPixel(x0 + x, y0 - y, isOn);
                    this.DrawPixel(x0 + y, y0 - x, isOn);
                }
                if (cornername == CornerName.SW)    //0x8
                {
                    this.DrawPixel(x0 - y, y0 + x, isOn);
                    this.DrawPixel(x0 - x, y0 + y, isOn);
                }
                if (cornername == CornerName.NW)    //0x1
                {
                    this.DrawPixel(x0 - y, y0 - x, isOn);
                    this.DrawPixel(x0 - x, y0 - y, isOn);
                }
            }
        }

        /// <summary>
        ///     Copy a bitmap to the display.
        /// </summary>
        /// <remarks>
        /// Currently, this method only supports copying the bitmap over the contents of the display buffer.
        /// </remarks>
        /// <param name="x">Abscissa of the top left corner of the bitmap.</param>
        /// <param name="y">Ordinate of the top left corner of the bitmap.</param>
        /// <param name="width">Width of the bitmap in bytes.</param>
        /// <param name="height">Height of the bitmap in bytes.</param>
        /// <param name="bitmap">Bitmap to transfer</param>
        public void DrawBitmap(int x, int y, int width, int height, byte[] bitmap, bool isOn)
        {
            if ((width * height) != bitmap.Length)
            {
                throw new ArgumentException("Width and height do not match the bitmap size.");
            }
            for (var ordinate = 0; ordinate < height; ordinate++)
            {
                for (var abscissa = 0; abscissa < width; abscissa++)
                {
                    var b = bitmap[(ordinate * width) + abscissa];
                    byte mask = 0x01;
                    for (var pixel = 0; pixel < 8; pixel++)
                    {
                        bool isPixelOn = (b & mask) > 0;
                        if (isOn)
                        {
                            this.DrawPixel((x + (8 * abscissa) + pixel), (y + ordinate), isPixelOn);
                        }
                        else
                        {
                            this.DrawPixel((x + (8 * abscissa) + pixel), (y + ordinate), !isPixelOn);
                        }

                        mask <<= 1;
                    }
                }
            }
        }

        #region --- Draw Text ---

        /// <summary>
        /// Draw a text message on the display using the current font.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="isOn">Color of the text.</param>
        public void DrawText(string text, bool isOn, bool updateDisplay = false)
        {
            this.DrawText(this.CursorPosition.X, this.CursorPosition.Y, text, isOn, updateDisplay);
        }

        /// <summary>
        /// Draw a text message on the display using the current font.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="isOn">If the text is on pixels (or inverted / off)</param>
        public void DrawText(int x, int y, string text, bool isOn, bool updateDisplay = false)
        {
            if (this.CurrentFont == null) { throw new Exception("CurrentFont must be set before calling DrawText."); }

            byte[] bitMap = this.GetBytesForTextBitmap(text);

            this.DrawBitmap(x, y, (bitMap.Length / this.CurrentFont.Height), this.CurrentFont.Height, bitMap, isOn);

            // figure out where the cursor should land after writing
            this.CursorPosition = new Point(x + text.Length * this.CurrentFont.Width, y);

            if (updateDisplay) { this.Show(); }
        }

        private byte[] GetBytesForTextBitmap(string text)
        {
            byte[] bitMap;

            if (this.CurrentFont.Width == 8) //just copy bytes
            {
                bitMap = new byte[text.Length * this.CurrentFont.Height * this.CurrentFont.Width / 8];

                for (int i = 0; i < text.Length; i++)
                {
                    var characterMap = this.CurrentFont[text[i]];

                    for (int segment = 0; segment < this.CurrentFont.Height; segment++)
                    {
                        bitMap[i + (segment * text.Length)] = characterMap[segment];
                    }
                }
            }
            else if (this.CurrentFont.Width == 4)
            {
                var len = (text.Length + text.Length % 2) / 2;
                bitMap = new byte[len * this.CurrentFont.Height];
                byte[] characterMap1, characterMap2;

                for (int i = 0; i < len; i++)
                {
                    characterMap1 = this.CurrentFont[text[2 * i]];
                    characterMap2 = (i * 2 + 1 < text.Length) ? this.CurrentFont[text[2 * i + 1]] : this.CurrentFont[' '];

                    for (int j = 0; j < characterMap1.Length; j++)
                    {
                        bitMap[i + (j * 2 + 0) * len] = (byte)((characterMap1[j] & 0x0F) | (characterMap2[j] << 4));
                        bitMap[i + (j * 2 + 1) * len] = (byte)((characterMap1[j] >> 4) | (characterMap2[j] & 0xF0));
                    }
                }
            }
            else
            {
                throw new Exception("Font width must be 4, or 8");
            }
            return bitMap;
        }


        #endregion


        /// <summary>
        /// Start the display scrollling in the specified direction.
        /// </summary>
        /// <param name="direction">Direction that the display should scroll.</param>
        public void StartScrolling(ScrollDirection direction)
        {
            this.StartScrolling(direction, 0x00, 0xff);
        }

        /// <summary>
        /// Start the display scrolling.
        /// </summary>
        /// <remarks>
        /// In most cases setting startPage to 0x00 and endPage to 0xff will achieve an acceptable scrolling effect.
        /// </remarks>
        /// <param name="direction">Direction that the display should scroll.</param>
        /// <param name="startPage">Start page for the scroll.</param>
        /// <param name="endPage">End oage for the scroll.</param>
        public void StartScrolling(ScrollDirection direction, byte startPage, byte endPage)
        {
            this.StopScrolling();
            byte[] commands;
            if ((direction == ScrollDirection.Left) || (direction == ScrollDirection.Right))
            {
                commands = new byte[] { 0x26, 0x00, startPage, 0x00, endPage, 0x00, 0xff, 0x2f };
                if (direction == ScrollDirection.Left)
                {
                    commands[0] = 0x27;
                }
            }
            else
            {
                byte scrollDirection;
                if (direction == ScrollDirection.LeftAndVertical)
                {
                    scrollDirection = 0x2a;
                }
                else
                {
                    scrollDirection = 0x29;
                }
                commands = new byte[]  { 0xa3, 0x00, (byte)this.Height, scrollDirection, 0x00, startPage, 0x00, endPage, 0x01, 0x2f };
            }
            this.SendCommands(commands);
        }

        /// <summary>
        ///     Turn off scrolling.
        /// </summary>
        /// <remarks>
        ///     Datasheet states that scrolling must be turned off before changing the
        ///     scroll direction in order to prevent RAM corruption.
        /// </remarks>
        public void StopScrolling()
        {
            this.SendCommand(0x2e);
        }

        #region --- Setup Sequences ---

        /// <summary>
        ///     Sequence of bytes that should be sent to a 128x64 OLED display to setup the device.
        /// </summary>
        private readonly byte[] _oled128x64SetupSequence = { 0xae, 0xd5, 0x80, 0xa8, 0x3f, 0xd3, 0x00, 0x40 | 0x0, 0x8d, 0x14, 0x20, 0x00, 0xa0 | 0x1, 0xc8, 0xda, 0x12, 0x81, 0xcf, 0xd9, 0xf1, 0xdb, 0x40, 0xa4, 0xa6, 0xaf };

        /// <summary>
        /// Sequence of bytes that should be sent to a 128x32 OLED display to setup the device.
        /// </summary>
        private readonly byte[] _oled128x32SetupSequence = { 0xae, 0xd5, 0x80, 0xa8, 0x1f, 0xd3, 0x00, 0x40 | 0x0, 0x8d, 0x14, 0x20, 0x00, 0xa0 | 0x1, 0xc8, 0xda, 0x02, 0x81, 0x8f, 0xd9, 0x1f, 0xdb, 0x40, 0xa4, 0xa6, 0xaf };

        /// <summary>
        /// Sequence of bytes that should be sent to a 96x16 OLED display to setup the device.
        /// </summary>
        private readonly byte[] _oled96x16SetupSequence = { 0xae, 0xd5, 0x80, 0xa8, 0x1f, 0xd3, 0x00, 0x40 | 0x0, 0x8d, 0x14, 0x20, 0x00, 0xa0 | 0x1, 0xc8, 0xda, 0x02, 0x81, 0xaf, 0xd9, 0x1f, 0xdb, 0x40, 0xa4, 0xa6, 0xaf };
        /// <summary>
        ///     Sequence of bytes that should be sent to a 64x48 OLED display to setup the device.
        /// </summary>
        private readonly byte[] _oled64x48SetupSequence = { 0xae, 0xd5, 0x80, 0xa8, 0x3f, 0xd3, 0x00, 0x40 | 0x0, 0x8d, 0x14, 0x20, 0x00, 0xa0 | 0x1, 0xc8, 0xda, 0x12, 0x81, 0xcf, 0xd9, 0xf1, 0xdb, 0x40, 0xa4, 0xa6, 0xaf };

        #endregion

        /// <summary>
        /// Scroll direction.
        /// </summary>
        public enum ScrollDirection
        {
            Left,
            Right,
            RightAndVertical,
            LeftAndVertical
        }

        /// <summary>
        /// Display Type
        /// </summary>
        public enum DisplayType
        {
            OLED128x64,     //0.91" screen
            OLED128x32,
            OLED64x48,
            OLED96x16,
        }

        public enum CornerName
        {
            NE,
            SE,
            SW,
            NW
        }
    }
}
