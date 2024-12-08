// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Device.Gpio;
using System.Device.I2c;

using IotDevice.Ssd13xx.Commands.Ssd1306Commands;

namespace IotDevice.Ssd13xx
{
    /// <summary>
    /// Represents base class for SSD13xx OLED displays.
    /// </summary>
    public abstract class Ssd13xx : IDisposable
    {
        /// <summary>
        /// Sequence of bytes that should be sent to a 128x64 OLED display to setup the device.
        /// First byte is the command byte 0x00.
        /// </summary>
        private readonly byte[] _oled128x64Init =
        [
            0x00,       // is command
            0xae,       // turn display off
            0xd5, 0x80, // set display clock divide ratio/oscillator,  set ratio = 0x80
            0xa8, 0x3f, // set multiplex ratio 0x00-0x3f        
            0xd3, 0x00, // set display offset 0x00-0x3f, no offset = 0x00
            0x40 | 0x0, // set display start line 0x40-0x7F
            0x8d, 0x14, // set charge pump,  enable  = 0x14  disable = 0x10
            0x20, 0x00, // 0x20 set memory address mode,  0x0 = horizontal addressing mode
            0xa0 | 0x1, // set segment re-map
            0xc8,       // set com output scan direction
            0xda, 0x12, // set COM pins HW configuration
            0x81, 0xcf, // set contrast control for BANK0, extVcc = 0x9F,  internal = 0xcf
            0xd9, 0xf1, // set pre-charge period  to 0xf1,  if extVcc then set to 0x22
            0xdb,       // set VCOMH deselect level
            0x40,       // set display start line
            0xa4,       // set display ON
            0xa6,       // set normal display
            0xaf        // turn display on 0xaf
        ];

        /// <summary>
        /// Sequence of bytes that should be sent to a 128x64 OLED display to setup the device.
        /// First byte is the command byte 0x00.
        /// </summary>
        private readonly byte[] _oled64x128Init =
        [
            0x00,       // is command
            0xd5,       // 1 clk div
            0xae,       // turn display off
            0xd5, 0x80, // set display clock divide ratio/oscillator,  set ratio = 0x80
            0xa8, 0x3f, // set multiplex ratio 0x00-0x3f        
            0xd3, 0x1f, // set display offset 0x00-0x3f, no offset = 0x00
            0x40 | 0x0, // set display start line 0x40-0x7F
            0x8d, 0x14, // set charge pump,  enable  = 0x14  disable = 0x10
            0x20, 0x00, // 0x20 set memory address mode,  0x0 = horizontal addressing mode
            0xa0 | 0x01, // set segment re-map
            0xc8,       // set com output scan direction
            0xda, 0x12, // set COM pins HW configuration
            0x81, 0xcf, // set contrast control for BANK0, extVcc = 0x9F,  internal = 0xcf
            0xd9, 0xf1, // set pre-charge period  to 0xf1,  if extVcc then set to 0x22
            0xdb,       // set VCOMH deselect level
            0x40,       // set display start line
            0xa4,       // set display ON
            0xa6,       // set normal display
            0xaf        // turn display on 0xaf
        ];

        /// <summary>
        /// Sequence of bytes that should be sent to a 128x32 OLED display to setup the device.
        /// First byte is the command byte 0x00.
        /// </summary>
        private readonly byte[] _oled128x32Init =
        [
            0x00,       // is command
            0xae,       // turn display off
            0xd5, 0x80, // set display clock divide ratio/oscillator,  set ratio = 0x80
            0xa8, 0x1f, // set multiplex ratio 0x00-0x1f        
            0xd3, 0x00, // set display offset 0x00-0x3f, no offset = 0x00
            0x40 | 0x0, // set display start line 0x40-0x7F
            0x8d, 0x14, // set charge pump,  enable  = 0x14  disable = 0x10
            0x20, 0x00, // 0x20 set memory address mode,  0x0 = horizontal addressing mode
            0xa0 | 0x1, // set segment re-map
            0xc8,       // set com output scan direction
            0xda, 0x02, // set COM pins HW configuration
            0x81, 0x8f, // set contrast control for BANK0, extVcc = 0x9F,  internal = 0xcf
            0xd9, 0xf1, // set pre-charge period  to 0xf1,  if extVcc then set to 0x22
            0xdb,       // set VCOMH deselect level
            0x40,       // set display start line
            0xa4,       // set display ON
            0xa6,       // set normal display
            0xaf        // turn display on 0xaf
        ];

        /// <summary>
        /// Sequence of bytes that should be sent to a 96x16 OLED display to setup the device.
        /// First byte is the command byte 0x00.
        /// </summary>
        private readonly byte[] _oled96x16Init =
        [
            0x00,       // is command
            0xae,       // turn display off
            0xd5, 0x80, // set display clock divide ratio/oscillator,  set ratio = 0x80
            0xa8, 0x1f, // set multiplex ratio 0x00-0x1f        
            0xd3, 0x00, // set display offset 0x00-0x3f, no offset = 0x00
            0x40 | 0x0, // set display start line 0x40-0x7F
            0x8d, 0x14, // set charge pump,  enable  = 0x14  disable = 0x10
            0x20, 0x00, // 0x20 set memory address mode,  0x0 = horizontal addressing mode
            0xa0 | 0x1, // set segment re-map
            0xc8,       // set com output scan direction
            0xda, 0x02, // set COM pins HW configuration
            0x81, 0xaf, // set contrast control for BANK0, extVcc = 0x9F,  internal = 0xcf
            0xd9, 0xf1, // set pre-charge period  to 0xf1,  if extVcc then set to 0x22
            0xdb,       // set VCOMH deselect level
            0x40,       // set display start line
            0xa4,       // set display ON
            0xa6,       // set normal display
            0xaf        // turn display on 0xaf
        ];

        // Multiply of screen resolution plus single command byte.
        private byte[] _genericBuffer;
        private byte[] _pageData;

        /// <summary>
        /// Gets or sets underlying I2C device.
        /// </summary>
        protected I2cDevice I2cDevice { get; set; }

        /// <summary>
        /// Gets or sets Screen rotation.  0 = no rotation, 1 = 90, 2 = 180, 3 = 270.
        /// </summary>
        public DisplayOrientation Orientation { get; set; } = DisplayOrientation.Landscape;

        /// <summary>
        /// Gets or sets Screen Resolution Width in Pixels.
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// Gets or sets Screen Resolution Height in Pixels.
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        /// Gets or sets Screen data pages.
        /// </summary>
        public byte Pages { get; init; }

        /// <summary>
        /// Gets or sets Font to use.
        /// </summary>
        public Fonts.IFont Font { get; set; } = new Fonts.BasicFont8x8();    //must be at least something


        private GpioController? GpioController { get; set; }
        private int ResetPin { get; set; } = -1; //set to invalid pin
        private bool ShouldDispose { get; set; }
        private bool IsDisposed { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ssd13xx" /> class.
        /// </summary>
        /// <param name="i2cDevice">I2C device used to communicate with the device.</param>
        /// <param name="resolution">Screen resolution to use for device init.</param>
        /// <param name="orientation">Orientation of the displayn.</param>
        /// <param name="resetPin">Reset pin (some displays might be wired to share the microcontroller's reset pin).</param>
        /// <param name="gpioController">Gpio Controller if using a reset pin.</param>
        /// <param name="shouldDispose">True to dispose the GpioController.</param>
        public Ssd13xx(I2cDevice i2cDevice, DisplayResolution resolution = DisplayResolution.OLED128x64, DisplayOrientation orientation = DisplayOrientation.Landscape, int resetPin = -1, GpioController? gpioController = null, bool shouldDispose = true)
        {
            ArgumentNullException.ThrowIfNull(i2cDevice, nameof(i2cDevice));
            if(resetPin >= 0 && gpioController == null) { throw new ArgumentNullException("GPIO controller required if using a reset pin."); }

            this.I2cDevice = i2cDevice;

            // reset pin provided?                        
            if (resetPin >= 0)
            {
                this.ResetPin = resetPin;
                this.GpioController = gpioController;
                this.Reset();
            }

            this.ShouldDispose = shouldDispose || (gpioController == null);
                       
            this.Orientation = orientation;

            switch (resolution)
            {
                case DisplayResolution.OLED128x64:
                    {
                        this.Width = 128;
                        this.Height = 64;
                        this.I2cDevice.Write(_oled128x64Init);
                        break;
                    }
                case DisplayResolution.OLED64x128:
                    {
                        this.Width = 64;
                        this.Height = 128;
                        this.I2cDevice.Write(_oled64x128Init);
                        break;
                    }
                case DisplayResolution.OLED128x32:
                    {
                        this.Width = 128;
                        this.Height = 32;
                        this.I2cDevice.Write(_oled128x32Init);
                        break;
                    }
                case DisplayResolution.OLED96x16:
                    {
                        this.Width = 96;
                        this.Height = 16;
                        this.I2cDevice.Write(_oled96x16Init);
                        break;
                    }
            }

            this.Pages = (byte)(this.Height / 8);

            // Adding 4 bytes make it SSH1106 IC OLED compatible
            _genericBuffer = new byte[(this.Pages * this.Width) + 4];
            _pageData = new byte[this.Width + 1];
        }

        /// <summary>
        /// Verifies value is within a specific range.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="start">Starting value of range.</param>
        /// <param name="end">Ending value of range.</param>
        /// <returns>Determines if value is within range.</returns>
        internal static bool InRange(uint value, uint start, uint end)
        {
            return (value - start) <= (end - start);
        }

        /// <summary>
        /// Send a command to the display controller.
        /// </summary>
        /// <param name="command">The command to send to the display controller.</param>
        public abstract void SendCommand(Commands.ISharedCommand command);

        /// <summary>
        /// Send data to the display controller.
        /// </summary>
        /// <param name="data">The data to send to the display controller.</param>
        public virtual void SendData(Span<Byte> data)
        {
            if (data.IsEmpty)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Span<Byte> writeBuffer = SliceGenericBuffer(data.Length + 1);

            writeBuffer[0] = 0x40; // Control byte.
            data.CopyTo(writeBuffer.Slice(1));
            this.I2cDevice.Write(writeBuffer);
        }

        /// <summary>
        /// Acquires span of specific length pointing to the command buffer.
        /// If length of the command buffer is too small it will be reallocated.
        /// </summary>
        /// <param name="length">Requested length.</param>
        /// <returns>Span of bytes pointing to the command buffer.</returns>
        protected Span<Byte> SliceGenericBuffer(int length) => SliceGenericBuffer(0, length);

        /// <summary>
        /// Acquires span of specific length at specific position in command buffer.
        /// If length of the command buffer is too small it will be reallocated.
        /// </summary>
        /// <param name="start">Start index of the requested span.</param>
        /// <param name="length">Requested length.</param>
        /// <returns>Span of bytes pointing to the command buffer.</returns>
        protected Span<Byte> SliceGenericBuffer(int start, int length)
        {
            if (_genericBuffer.Length < length)
            {
                var newBuffer = new byte[_genericBuffer.Length * 2];
                _genericBuffer.CopyTo(newBuffer, 0);
                _genericBuffer = newBuffer;
            }

            return _genericBuffer;
        }

        /// <summary>
        /// Copies buffer content directly to display buffer.
        /// Y and height must be byte aligned because buffer will 
        /// be copied without any logical operations on existing content.
        /// </summary>
        /// <param name="x">The X coordinate on the screen.</param>
        /// <param name="y">The Y coordinate on the screen.</param>
        /// <param name="width">Width of buffer content in pixels.</param>
        /// <param name="height">Height of buffer content in pixels.</param>
        /// <param name="buffer">Data to copy. Buffer size must be equal to height * width / 8.</param>
        public void DrawDirectAligned(int x, int y, int width, int height, byte[] buffer)
        {
            if ((y % 8) != 0)
            {
                throw new ArgumentException("Y must be aligned to byte boundary.");
            }

            if ((height % 8) != 0)
            {
                throw new ArgumentException("Height must be aligned to byte boundary.");
            }

            var dataHeightInBytes = height / 8;
            if ((dataHeightInBytes * width) != buffer.Length)
            {
                throw new ArgumentException("Width and height do not match the bitmap size.");
            }

            var genericBufferIdx = ((y / 8) * this.Width) + x;
            var srcIdx = 0;
            for (int i = 0; i < dataHeightInBytes; i++)
            {
                Array.Copy(buffer, srcIdx, _genericBuffer, genericBufferIdx, width);
                srcIdx += width;
                genericBufferIdx += this.Width;
            }
        }

        /// <summary>
        /// Clears portion of display via writing 0x00 directly to display buffer.
        /// Y and height must be byte aligned because bytes will 
        /// be written without any logical operations on existing content.
        /// </summary>
        /// <param name="x">The X coordinate on the screen.</param>
        /// <param name="y">The Y coordinate on the screen.</param>
        /// <param name="width">Width of area in pixels.</param>
        /// <param name="height">Height of area in pixels.</param>
        public void ClearDirectAligned(int x, int y, int width, int height)
        {
            if ((y % 8) != 0)
            {
                throw new ArgumentException("Y must be aligned to byte boundary.");
            }

            if ((height % 8) != 0)
            {
                throw new ArgumentException("Height must be aligned to byte boundary.");
            }

            var dataHeightInBytes = height / 8;
            var genericBufferIdx = ((y / 8) * Width) + x;
            for (int i = 0; i < dataHeightInBytes; i++)
            {
                Array.Clear(_genericBuffer, genericBufferIdx, width);
                genericBufferIdx += Width;
            }
        }

        /// <summary>
        /// In-place swap of a and b values without the use of a temp variable.
        /// </summary>
        /// <param name="a">1st variable to be swapped.</param>
        /// <param name="b">2nd variable to be swapped.</param>
        private void Swap(ref int a, ref int b)
        {
            a ^= b;
            b ^= a;
            a ^= b;     
        }

        /// <summary>
        /// Draws a pixel on the screen.
        /// </summary>
        /// <param name="x">The X coordinate on the screen.</param>
        /// <param name="y">The Y coordinate on the screen.</param>
        /// <param name="inverted">Indicates if color to be used turn the pixel on, or leave off.</param>
        public void DrawPixel(int x, int y, bool inverted = true)
        {
            DisplayOrientation defaultOrientation = DisplayOrientation.Landscape;
            if (this.Width < this.Height)
            {
                defaultOrientation = DisplayOrientation.Portrait;
            }

            int rotation = 0;

            if (defaultOrientation != this.Orientation)
            {
                rotation = Math.Abs((int)defaultOrientation - (int)this.Orientation) * 90;

                switch (rotation)
                {
                    case 90:
                        {
                            Swap(ref x, ref y);
                            x = this.Width - x - 1;
                            break;
                        }
                    case 180:
                        {
                            x = this.Width - x - 1;
                            y = this.Height - y - 1;
                            break;
                        }
                    case 270:
                        {
                            Swap(ref x, ref y);
                            y = this.Height - y - 1;
                            break;
                        }
                }
            }

            if (x >= this.Width || x < 0 || y >= this.Height || y < 0)
            {
                return;
            }

            // x specifies the column
            int idx = x + ((y / 8) * this.Width);

            if (inverted)
            {
                _genericBuffer[idx] |= (byte)(1 << (y & 7));
            }
            else
            {
                _genericBuffer[idx] &= (byte)~(1 << (y & 7));
            }
        }

        /// <summary>
        /// Draws a horizontal line.
        /// </summary>
        /// <param name="x">X coordinate starting of the line.</param>
        /// <param name="y">Y coordinate starting of line.</param>
        /// <param name="length">Line length.</param>
        /// <param name="inverted">Turn the pixel on (true) or off (false).</param>
        public void DrawLineHorizontal(int x, int y, int length, bool inverted = true)
        {
            for (var i = x; (i - x) < length; i++)
            {
                this.DrawPixel(i, y, inverted);
            }
        }

        /// <summary>
        /// Draws a vertical line.
        /// </summary>
        /// <param name="x">X coordinate starting of the line.</param>
        /// <param name="y">Y coordinate starting of line.</param>
        /// <param name="length">Line length.</param>
        /// <param name="inverted">Turn the pixel on (true) or off (false).</param>
        public void DrawLineVertical(int x, int y, int length, bool inverted = true)
        {
            for (var i = y; (i - y) < length; i++)
            {
                this.DrawPixel(x, i, inverted);
            }
        }

        /// <summary>
        /// Draw a rectangle
        /// </summary>
        /// <param name="x">X coordinate starting of the top left.</param>
        /// <param name="y">Y coordinate starting of the top left.</param>
        /// <param name="width">Width of rectabgle in pixels.</param>
        /// <param name="height">Height of rectangle in pixels.</param>
        /// <param name="inverted">Turn the pixel on (true) or off (false).</param>
        public void DrawRectangle(int x, int y, int width, int height, bool inverted = true)
        {
            this.DrawLineHorizontal(x, y, width, inverted);    //Top
            this.DrawLineHorizontal(x, y + height - 1, width, inverted);    //Bottom

            this.DrawLineVertical(x, y, height, inverted); //Left
            this.DrawLineVertical(x + width - 1, y, height, inverted); //Right
        }

        /// <summary>
        /// Draws a rectangle that is solid/filled.
        /// </summary>
        /// <param name="x">X coordinate starting of the top left.</param>
        /// <param name="y">Y coordinate starting of the top left.</param>
        /// <param name="width">Width of rectabgle in pixels.</param>
        /// <param name="height">Height of rectangle in pixels.</param>
        /// <param name="inverted">Turn the pixel on (true) or off (false).</param>
        public void DrawRectangleFilled(int x, int y, int width, int height, bool inverted = true)
        {
            for (int i = 0; i <= height; i++)
            {
                this.DrawLineHorizontal(x, y + i, width, inverted);
            }
        }

        /// <summary>
        /// Draw a rectangle with rounded corners
        /// </summary>
        /// <param name="x">X coordinate starting of the top left.</param>
        /// <param name="y">Y coordinate starting of the top left.</param>
        /// <param name="width">Width of rectabgle in pixels.</param>
        /// <param name="height">Height of rectangle in pixels.</param>
        /// <param name="radius"></param>
        /// <param name="inverted">Turn the pixel on (true) or off (false).</param>
        public void DrawRectangleRounded(int x, int y, int width, int height, int radius, bool inverted = true)
        {
            // draw the non-curved line segments
            this.DrawLineHorizontal(x + radius, y, width - 2 * radius, inverted); // Top
            this.DrawLineHorizontal(x + radius, y + height - 1, width - 2 * radius, inverted); // Bottom

            this.DrawLineVertical(x, y + radius, height - 2 * radius, inverted); // Left
            this.DrawLineVertical(x + width - 1, y + radius, height - 2 * radius, inverted); // Right

            // draw four corners
            this.DrawCornerHelper(x + radius, y + radius, radius, CornerName.NW, inverted);                             //1
            this.DrawCornerHelper(x + width - radius - 1, y + radius, radius, CornerName.SE, inverted);                 //2
            this.DrawCornerHelper(x + width - radius - 1, y + height - radius - 1, radius, CornerName.NE, inverted);    //4
            this.DrawCornerHelper(x + radius, y + height - radius - 1, radius, CornerName.SW, inverted);                //8
        }

        private void DrawCornerHelper(int x0, int y0, int r, CornerName cornername, bool inverted)
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

                if (cornername == CornerName.NE)
                {
                    this.DrawPixel(x0 + x, y0 + y, inverted);
                    this.DrawPixel(x0 + y, y0 + x, inverted);
                }
                if (cornername == CornerName.SE)
                {
                    this.DrawPixel(x0 + x, y0 - y, inverted);
                    this.DrawPixel(x0 + y, y0 - x, inverted);
                }
                if (cornername == CornerName.SW)
                {
                    this.DrawPixel(x0 - y, y0 + x, inverted);
                    this.DrawPixel(x0 - x, y0 + y, inverted);
                }
                if (cornername == CornerName.NW)
                {
                    this.DrawPixel(x0 - y, y0 - x, inverted);
                    this.DrawPixel(x0 - x, y0 - y, inverted);
                }
            }
        }


        /// <summary>
        /// Displays the  1 bit bit map.
        /// </summary>
        /// <param name="x">The X coordinate on the screen.</param>
        /// <param name="y">The Y coordinate on the screen.</param>
        /// <param name="width">Width in bytes.</param>
        /// <param name="height">Height in bytes.</param>
        /// <param name="bitmap">Bitmap to display.</param>
        /// <param name="size">Drawing size, normal = 1, larger use 2,3 etc.</param>
        public void DrawBitmap(int x, int y, int width, int height, byte[] bitmap, byte size = 1)
        {
            if ((width * height) != bitmap.Length)
            {
                throw new ArgumentException("Width and height do not match the bitmap size.");
            }

            byte mask = 0x01;
            byte b;

            for (var yO = 0; yO < height; yO++)
            {
                for (var xA = 0; xA < width; xA++)
                {
                    b = bitmap[(yO * width) + xA];

                    for (var pixel = 0; pixel < 8; pixel++)
                    {
                        if (size == 1)
                        {
                            this.DrawPixel(x + (8 * xA) + pixel, y + yO, (b & mask) > 0);
                        }
                        else
                        {
                            this.DrawRectangleFilled((x + (8 * xA) + pixel) * size, ((y / size) + yO) * size, size, size, (b & mask) > 0);
                        }

                        mask <<= 1;
                    }

                    // Reset each time to support SSH1106 OLEDs
                    mask = 0x01;
                }
            }
        }

        /// <summary>
        /// Writes a text message on the screen with font in use.
        /// </summary>
        /// <param name="x">The x pixel-coordinate on the screen.</param>
        /// <param name="y">The y pixel-coordinate on the screen.</param>
        /// <param name="str">Text string to display.</param>
        /// <param name="size">Text size, normal = 1, larger use 2,3, 4 etc.</param>
        /// <param name="center">Indicates if text should be centered if possible.</param>
        /// <seealso cref="Write"/>
        public void DrawString(int x, int y, string str, byte size = 1, bool center = false)
        {
            // nothing to do?
            if (string.IsNullOrEmpty(str)) { return; }

            int w = this.Width;
            
            if (this.Width < this.Height && (this.Orientation == DisplayOrientation.Landscape || this.Orientation == DisplayOrientation.Landscape180))
            {
                w = Height;
            }

            if (center)
            {
                int padSize = (((w / size) / this.Font.Width) - str.Length) / 2;
                if (padSize > 0)
                {
                    str = str.PadLeft(str.Length + padSize);
                }
            }

            byte[] bitMap = Font.Width > 8 ? this.GetDoubleTextBytes(str) : this.GetTextBytes(str);

            this.DrawBitmap(x, y, bitMap.Length / this.Font.Height, this.Font.Height, bitMap, size);
        }

        /// <summary>
        /// Writes a text message on the screen with font in use.
        /// </summary>
        /// <param name="x">The x text-coordinate on the screen.</param>
        /// <param name="y">The y text-coordinate on the screen.</param>
        /// <param name="str">Text string to display.</param>
        /// <param name="size">Text size, normal = 1, larger use 2,3, 4 etc.</param>
        /// <param name="center">Indicates if text should be centered if possible.</param>
        /// <seealso cref="DrawString"/>
        public void Write(int x, int y, string str, byte size = 1, bool center = false)
        {
            this.DrawString(x * this.Font.Width, y * this.Font.Height, str, size, center);
        }

        /// <summary>
        /// Get the bytes to be drawn on the screen for text, from the font.
        /// </summary>
        /// <param name="text">Strint to be shown on the screen.</param>
        /// <returns>The bytes to be drawn using current font.</returns>
        private byte[] GetTextBytes(string text)
        {
            byte[] bitMap;

            if (this.Font.Width == 8)
            {
                bitMap = new byte[text.Length * this.Font.Height * this.Font.Width / 8];

                for (int i = 0; i < text.Length; i++)
                {
                    var characterMap = this.Font[text[i]];

                    for (int segment = 0; segment < this.Font.Height; segment++)
                    {
                        bitMap[i + (segment * text.Length)] = segment < characterMap.Length ? characterMap[segment] : byte.MinValue;
                    }
                }
            }
            else
            {
                throw new Exception("Font width must be 8");
            }

            return bitMap;
        }

        /// <summary>
        /// Get the bytes to be drawn on the screen for double-byte text, from the font
        /// e.g. 功夫, カンフー, 쿵후.
        /// </summary>
        /// <param name="text">Strint to be shown on the screen.</param>
        /// <returns>The bytes to be drawn using current font.</returns>
        private byte[] GetDoubleTextBytes(string text)
        {
            byte[] bitMap;

            if (this.Font.Width == 16)
            {
                var charCount = text.Length * 2;
                bitMap = new byte[charCount * this.Font.Height * this.Font.Width / 16];

                byte[] characterMap = [];
                for (int i = 0; i < charCount; i++)
                {
                    var even = i == 0 || i % 2 == 0;
                    if (even)
                    {
                        characterMap = this.Font[text[i / 2]];
                    }

                    var list = new System.Collections.ArrayList();
                    for (int idx = even ? 0 : 1; idx < characterMap.Length; idx += 2)
                    {
                        list.Add(characterMap[idx]);
                    }

                    for (int segment = 0; segment < Font.Height; segment++)
                    {
                        bitMap[i + (segment * charCount)] = segment < list.Count ? (byte)list[segment]! : byte.MinValue;
                    }
                }
            }
            else
            {
                throw new Exception("Double-byte characters font width must be 16");
            }

            return bitMap;
        }

        /// <summary>
        /// Displays the information on the screen using page mode.
        /// </summary>
        public void Show()
        {
            for (byte i = 0; i < Pages; i++)
            {
                _pageCmd[1] = (byte)(PageAddress.Page0 + i); // page number
                this.I2cDevice.Write(_pageCmd);

                _pageData[0] = 0x40; // is data
                Array.Copy(_genericBuffer, i * this.Width, _pageData, 1, this.Width);
                this.I2cDevice.Write(_pageData);
            }
        }

        /// <summary>
        /// Clears the screen buffer and calls 'Display()'.
        /// </summary>
        public void ClearScreen(bool updateDisplay = false)
        {
            Array.Clear(_genericBuffer, 0, _genericBuffer.Length);

            if (updateDisplay)
            {
                this.Show();
            }            
        }

        /// <summary>
        /// Page mode output command bytes.
        /// </summary>
        private byte[] _pageCmd =
        [
            0x00, // is command
            0xB0, // page address (B0-B7)
            0x00, // lower columns address =0
            0x10, // upper columns address =0
        ];

        /// <summary>
        /// Reset display controller.
        /// </summary>        
        private void Reset()
        {
            // nothing to do?
            if (this.ResetPin < 0 || this.GpioController == null) { return; }

            GpioPin rstPin = this.GpioController.OpenPin(this.ResetPin, PinMode.Output);
            rstPin.Write(PinValue.High);
            Thread.Sleep(1);                // VDD goes high at start, pause for 1 ms            
            rstPin.Write(PinValue.Low);     // Bring reset low
            Thread.Sleep(10);               // Wait 10 ms
            rstPin.Write(PinValue.High);    // Bring out of reset
            Thread.Sleep(1);
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>
        /// <param name="disposing">Should dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed) { return; }

            if (disposing)
            {
                if (this.ResetPin >= 0 && this.GpioController != null)
                {
                    this.GpioController.ClosePin(ResetPin);
                    if (this.ShouldDispose)
                    {
                        this.GpioController.Dispose();
                        this.GpioController = null;
                    }
                }

                this.I2cDevice.Dispose();

                this.IsDisposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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