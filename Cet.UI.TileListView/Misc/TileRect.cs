/*
 * Copyright 2016 by Mario Vernari, Cet Electronics
 *
 * Licensed under the MIT License
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cet.UI.TileListView
{
    /// <summary>
    /// "Rect" surrogate for best geometry manipulation
    /// </summary>
    /// <remarks>
    /// This class privileges the rectangle as four-walled shape,
    /// than the ordinary structure exposing position and size.
    /// The usage of the single edges simplifies the interaction.
    /// </remarks>
    internal sealed class TileRect
    {
        internal TileRect(
            double left,
            double top,
            double right,
            double bottom
            )
        {
            if (left > right)
            {
                throw new ArgumentException();
            }

            if (top > bottom)
            {
                throw new ArgumentException();
            }

            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }


        public double Left { get; private set; }
        public double Top { get; private set; }
        public double Right { get; private set; }
        public double Bottom { get; private set; }


        public double Width
        {
            get { return this.Right - this.Left; }
        }


        public double Height
        {
            get { return this.Bottom - this.Top; }
        }


        public void SetLeft(double value, PointerMovingRange limit)
        {
            this.Left = Math.Min(
                limit.Coerce(value),
                this.Right
                );
        }


        public void SetTop(double value, PointerMovingRange limit)
        {
            this.Top = Math.Min(
                limit.Coerce(value),
                this.Bottom
                );
        }


        public void SetRight(double value, PointerMovingRange limit)
        {
            this.Right = Math.Max(
                limit.Coerce(value),
                this.Left
                );
        }


        public void SetBottom(double value, PointerMovingRange limit)
        {
            this.Bottom = Math.Max(
                limit.Coerce(value),
                this.Top
                );
        }


        public override string ToString()
        {
            return $"TileRect: L={Left}; T={Top}; R={Right}; B={Bottom}";
        }


        public TileRect Clone()
        {
            return new TileRect(
                this.Left,
                this.Top,
                this.Right,
                this.Bottom
                );
        }


        public static TileRect FromRect(
            double left,
            double top,
            double width,
            double height
            )
        {
            return new TileRect(
                left,
                top,
                left + width,
                top + height
                );
        }

    }
}
