using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace Svg
{
    /// <summary>
    /// Represents a unit in an Scalable Vector Graphics document.
    /// </summary>
    [TypeConverter(typeof(SvgUnitConverter))]
    public struct SvgUnit
    {
        private SvgUnitType _type;
        private float _value;
        private bool _isEmpty;
        private float? _deviceValue;

        public static readonly SvgUnit Empty = new SvgUnit();

        /// <summary>
        /// Gets a value to determine whether the unit is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return this._isEmpty; }
        }

        /// <summary>
        /// Gets the value of the unit.
        /// </summary>
        public float Value
        {
            get { return this._value; }
        }

        public SvgUnitType Type
        {
            get { return this._type; }
        }

        public float ToDeviceValue()
        {
            return this.ToDeviceValue(null);
        }

        public float ToDeviceValue(ISvgStylable styleOwner)
        {
            return this.ToDeviceValue(styleOwner, false);
        }

        public float ToDeviceValue(ISvgStylable styleOwner, bool vertical)
        {
            // If it's already been calculated
            if (this._deviceValue.HasValue)
            {
                return this._deviceValue.Value;
            }

            if (this._value == 0.0f)
            {
                this._deviceValue = 0.0f;
                return this._deviceValue.Value;
            }

            // http://www.w3.org/TR/CSS21/syndata.html#values
            // http://www.w3.org/TR/SVG11/coords.html#Units

            const float cmInInch = 2.54f;
            int ppi = SvgDocument.PPI;

            switch (this.Type)
            {
                case SvgUnitType.Centimeter:
                    _deviceValue = (float)((this.Value / cmInInch) * ppi);
                    break;
                case SvgUnitType.Inch:
                    _deviceValue = this.Value * ppi;
                    break;
                case SvgUnitType.Millimeter:
                    _deviceValue = (float)((this.Value / 10) / cmInInch) * ppi;
                    break;
                case SvgUnitType.Pica:
                    _deviceValue = ((this.Value * 12) / 72) * ppi;
                    break;
                case SvgUnitType.Point:
                    _deviceValue = (this.Value / 72) * ppi;
                    break;
                case SvgUnitType.Pixel:
                    _deviceValue = this.Value;
                    break;
                case SvgUnitType.User:
                    _deviceValue = this.Value;
                    break;
                case SvgUnitType.Percentage:
                    // Can't calculate if there is no style owner
                    if (styleOwner == null)
                    {
                        _deviceValue = this.Value;
                    }

                    // TODO : Support height percentages
                    System.Drawing.RectangleF size = styleOwner.Bounds;
                    _deviceValue = (((vertical) ? size.Height : size.Width) / 100) * this.Value;
                    break;
                default:
                    _deviceValue = this.Value;
                    break;
            }
            return this._deviceValue.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj.GetType() == typeof(SvgUnit)))
            {
                return false;
            }

            SvgUnit unit = (SvgUnit)obj;
            return (unit.Value == this.Value && unit.Type == this.Type);
        }

        public override string ToString()
        {
            string type = string.Empty;

            switch (this.Type)
            {
                case SvgUnitType.Pixel:
                    type = "px";
                    break;
                case SvgUnitType.Point:
                    type = "pt";
                    break;
                case SvgUnitType.Inch:
                    type = "in";
                    break;
                case SvgUnitType.Centimeter:
                    type = "cm";
                    break;
                case SvgUnitType.Millimeter:
                    type = "mm";
                    break;
                case SvgUnitType.Percentage:
                    type = "%";
                    break;
                case SvgUnitType.Em:
                    type = "em";
                    break;
            }

            return string.Concat(this.Value.ToString(), type);
        }

        public static implicit operator float(SvgUnit value)
        {
            return value.ToDeviceValue();
        }

        public static implicit operator SvgUnit(float value)
        {
            return new SvgUnit(value);
        }

        public SvgUnit(SvgUnitType type, float value)
        {
            this._type = type;
            this._value = value;
            this._isEmpty = (this._value == 0.0f);
            this._deviceValue = null;
        }

        public SvgUnit(float value)
        {
            this._value = value;
            this._type = SvgUnitType.User;
            this._isEmpty = (this._value == 0.0f);
            this._deviceValue = null;
        }
    }

    /// <summary>
    /// Defines the various types of unit an <see cref="SvgUnit"/> can be.
    /// </summary>
    public enum SvgUnitType
    {
        /// <summary>
        /// Indicates that the unit is in pixels.
        /// </summary>
        Pixel,
        /// <summary>
        /// Indicates that the unit is equal to the pt size of the current font.
        /// </summary>
        Em,
        /// <summary>
        /// Indicates that the unit is a percentage.
        /// </summary>
        Percentage,
        /// <summary>
        /// Indicates that the unit has no unit identifier and is a value in the current user coordinate system.
        /// </summary>
        User,
        /// <summary>
        /// Indicates the the unit is in inches.
        /// </summary>
        Inch,
        /// <summary>
        /// Indicates that the unit is in centimeters.
        /// </summary>
        Centimeter,
        /// <summary>
        /// Indicates that the unit is in millimeters.
        /// </summary>
        Millimeter,
        /// <summary>
        /// Indicates that the unit is in picas.
        /// </summary>
        Pica,
        /// <summary>
        /// Indicates that the unit is in points, the smallest unit of measure, being a subdivision of the larger <see cref="Pica"/>. There are 12 points in the <see cref="Pica"/>.
        /// </summary>
        Point
    }
}
