namespace BlazorTextEditor.ClassLib.Dimensions;

public class ElementDimensions
{
    public ElementDimensions()
    {
        DimensionAttributes.AddRange(new []
        {
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Width },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Height },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Left },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Right },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Top },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Bottom }
        });
    }

    public List<DimensionAttribute> DimensionAttributes { get; } = new();
    public ElementPositionKind ElementPositionKind { get; set; } = ElementPositionKind.Static;
}

public class DimensionAttribute
{
    public List<DimensionUnit> DimensionUnits { get; } = new();
    public DimensionAttributeKind DimensionAttributeKind { get; set; }
}

public enum DimensionAttributeKind
{
    Width,
    Height,
    Left,
    Right,
    Top,
    Bottom
}

public enum ElementPositionKind
{
    Static,
    Absolute,
    Fixed,
    Inherit,
    Relative,
    Revert,
    Sticky,
    Unset,
}

public class DimensionUnit
{
    public double Value { get; set; }
    public DimensionUnitKind DimensionUnitKind { get; set; }
}

public enum DimensionUnitKind
{
    Pixels,
    ViewportWidth,
    ViewportHeight,
    Percentage,
    RootCharacterWidth,
    RootCharacterHeight,
    CharacterWidth,
    CharacterHeight,
}

public enum DimensionOperatorKind
{
    Add,
    Subtract,
    Multiply,
    Divide
}







