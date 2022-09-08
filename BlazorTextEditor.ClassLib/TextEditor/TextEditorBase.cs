using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.ClassLib.Keyboard;

namespace BlazorTextEditor.ClassLib.TextEditor;

public class TextEditorBase
{
    public const string InitialInput = @"public class MyClass
{
    public void MyMethod()
    {
        return;
    }
}
";

    
    /// <summary>
    /// To get the ending position of RowIndex _rowEndingPositions[RowIndex]
    /// <br/><br/>
    /// _rowEndingPositions returns the start of the NEXT row
    /// </summary>
    private List<(int positionIndex, RowEndingKind rowEndingKind)> _rowEndingPositions = new();

    public TextEditorBase(string content)
    {
        var rowIndex = 0;
        var previousCharacter = '\0';
        
        for (var index = 0; index < content.Length; index++)
        {
            var character = content[index];
            
            if (character == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
            {
                _rowEndingPositions.Add((index + 1, RowEndingKind.CarriageReturn));
                rowIndex++;
            }
            else if (character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                if (previousCharacter == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
                {
                    var lineEnding = _rowEndingPositions[rowIndex - 1];
                    
                    _rowEndingPositions[rowIndex - 1] = (lineEnding.positionIndex + 1, RowEndingKind.CarriageReturnNewLine);
                }
                else
                {
                    _rowEndingPositions.Add((index + 1, RowEndingKind.NewLine));
                    rowIndex++;
                }
            }

            previousCharacter = character;

            Content.Add(new RichCharacter
            {
                Value = character,
                DecorationByte = default
            });
        }
        
        _rowEndingPositions.Add((content.Length, RowEndingKind.EndOfFile));
    }
    
    public List<RichCharacter> Content { get; set; } = new();
    
    public (int positionIndex, RowEndingKind rowEndingKind) GetStartOfRowTuple(int rowIndex)
    {
        return rowIndex > 0
            ? _rowEndingPositions[rowIndex - 1]
            : (0, RowEndingKind.StartOfFile);
    }

    /// <summary>
    /// Returns the Length of a row however it does not include the line ending characters by default.
    /// To include line ending characters the parameter <see cref="includeLineEndingCharacters"/> must be true.
    /// </summary>
    public int GetLengthOfRow(
        int rowIndex,
        bool includeLineEndingCharacters = false)
    {
        if (!_rowEndingPositions.Any())
            return 0;

        (int positionIndex, RowEndingKind rowEndingKind) startOfRowTupleInclusive = GetStartOfRowTuple(rowIndex);

        var endOfRowTupleExclusive = _rowEndingPositions[rowIndex];

        var lengthOfRowWithLineEndings = endOfRowTupleExclusive.positionIndex
                                         - startOfRowTupleInclusive.positionIndex;

        if (includeLineEndingCharacters)
            return lengthOfRowWithLineEndings;

        return lengthOfRowWithLineEndings - endOfRowTupleExclusive.rowEndingKind.AsCharacters().Length;
    }
    
    /// <param name="startingRowIndex">The starting index of the rows to return</param>
    /// <param name="count">count of 0 returns 0 rows. count of 1 returns the startingRowIndex.</param>
    public List<List<RichCharacter>> GetRows(int startingRowIndex, int count)
    {
        var rowCountAvailable = _rowEndingPositions.Count - startingRowIndex;

        var rowCountToReturn = count < rowCountAvailable
            ? count
            : rowCountAvailable;

        var endingRowIndexExclusive = startingRowIndex + rowCountToReturn;

        var rows = new List<List<RichCharacter>>();
        
        for (int i = startingRowIndex; 
             i < endingRowIndexExclusive;
             i++)
        {
            // Previous row's line ending position is this row's start.
            var startOfRowInclusive = GetStartOfRowTuple(i)
                .positionIndex;

            var endOfRowExclusive = _rowEndingPositions[i].positionIndex;

            var row = Content
                .Skip(startOfRowInclusive)
                .Take(endOfRowExclusive - startOfRowInclusive)
                .ToList();
            
            rows.Add(row);
        }
        
        return rows;
    }
}