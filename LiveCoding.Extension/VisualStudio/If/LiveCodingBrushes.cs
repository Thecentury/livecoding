using System.Windows.Media;
using LiveCoding.Extension.Extensions;

namespace LiveCoding.Extension.VisualStudio.If
{
	internal static class LiveCodingBrushes
	{
		public static readonly Brush TrueBrush = new SolidColorBrush( Color.FromArgb( 0xFF, 0xAB, 0xEA, 0xC6 ) ).AsFrozen();

		public static readonly Brush FalseBrush = new SolidColorBrush( Color.FromArgb( 0xFF, 0xFF, 0x85, 0x70 ) ).AsFrozen();

		public static Brush GetBrush( bool conditionValue )
		{
			return conditionValue ? TrueBrush : FalseBrush;
		}
	}
}