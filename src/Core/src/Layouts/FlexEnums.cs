using System;

namespace Microsoft.Maui.Layouts
{
	public enum FlexJustify
	{
		Start = Flex.Justify.Start,
		Center = Flex.Justify.Center,
		End = Flex.Justify.End,
		SpaceBetween = Flex.Justify.SpaceBetween,
		SpaceAround = Flex.Justify.SpaceAround,
		SpaceEvenly = Flex.Justify.SpaceEvenly,
	}

	public enum FlexPosition
	{
		Relative = Flex.Position.Relative,
		Absolute = Flex.Position.Absolute,
	}

	public enum FlexDirection
	{
		Column = Flex.Direction.Column,
		ColumnReverse = Flex.Direction.ColumnReverse,
		Row = Flex.Direction.Row,
		RowReverse = Flex.Direction.RowReverse,
	}

	public enum FlexAlignContent
	{
		Stretch = Flex.AlignContent.Stretch,
		Center = Flex.AlignContent.Center,
		Start = Flex.AlignContent.Start,
		End = Flex.AlignContent.End,
		SpaceBetween = Flex.AlignContent.SpaceBetween,
		SpaceAround = Flex.AlignContent.SpaceAround,
		SpaceEvenly = Flex.AlignContent.SpaceEvenly,
	}

	public enum FlexAlignItems
	{
		Stretch = Flex.AlignItems.Stretch,
		Center = Flex.AlignItems.Center,
		Start = Flex.AlignItems.Start,
		End = Flex.AlignItems.End,
		//Baseline = Flex.AlignItems.Baseline,
	}

	public enum FlexAlignSelf
	{
		Auto = Flex.AlignSelf.Auto,
		Stretch = Flex.AlignSelf.Stretch,
		Center = Flex.AlignSelf.Center,
		Start = Flex.AlignSelf.Start,
		End = Flex.AlignSelf.End,
		//Baseline = Flex.AlignSelf.Baseline,
	}

	public enum FlexWrap
	{
		NoWrap = Flex.Wrap.NoWrap,
		Wrap = Flex.Wrap.Wrap,
		Reverse = Flex.Wrap.WrapReverse,
	}

	public struct FlexBasis
	{
		bool _isLength;
		bool _isRelative;
		public static FlexBasis Auto = new();
		public float Length { get; }
		internal bool IsAuto => !_isLength && !_isRelative;
		internal bool IsRelative => _isRelative;
		public FlexBasis(float length, bool isRelative = false)
		{
			if (length < 0)
				throw new ArgumentException("should be a positive value", nameof(length));
			if (isRelative && length > 1)
				throw new ArgumentException("relative length should be in [0, 1]", nameof(length));
			_isLength = !isRelative;
			_isRelative = isRelative;
			Length = length;
		}

		public static implicit operator FlexBasis(float length)
		{
			return new FlexBasis(length);
		}
	}
}