using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.ClassLib.Keyboard;
using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using Microsoft.CodeAnalysis.Text;

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
    
    public const int TabWidth = 4;
    public const int GutterPaddingLeftInPixels = 5;
    public const int GutterPaddingRightInPixels = 5;

    /// <summary>
    /// To get the ending position of RowIndex _rowEndingPositions[RowIndex]
    /// <br/><br/>
    /// _rowEndingPositions returns the start of the NEXT row
    /// </summary>
    private readonly List<(int positionIndex, RowEndingKind rowEndingKind)> _rowEndingPositions = new();
    /// <summary>
    /// Provides exact position index of a tab character
    /// </summary>
    private readonly List<int> _tabKeyPositions = new();
    private readonly List<RichCharacter> _content = new();

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

            if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                _tabKeyPositions.Add(index);
                
            previousCharacter = character;

            _content.Add(new RichCharacter
            {
                Value = character,
                DecorationByte = default
            });
        }
        
        _rowEndingPositions.Add((content.Length, RowEndingKind.EndOfFile));
    }
    
    public int RowCount => _rowEndingPositions.Count;
    
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

            var row = _content
                .Skip(startOfRowInclusive)
                .Take(endOfRowExclusive - startOfRowInclusive)
                .ToList();
            
            rows.Add(row);
        }
        
        return rows;
    }

    public int GetTabsCountOnSameRowBeforeCursor(int rowIndex, int columnIndex)
    {
        var startOfRowPositionIndex = GetStartOfRowTuple(rowIndex)
            .positionIndex;

        var tabs = _tabKeyPositions
            .SkipWhile(positionIndex => positionIndex < startOfRowPositionIndex)
            .TakeWhile(positionIndex => positionIndex < startOfRowPositionIndex + columnIndex);

        return tabs.Count();
    }

    public TextEditorBase PerformEditTextEditorAction(EditTextEditorAction editTextEditorAction)
    {
        if (KeyboardKeyFacts.IsMetaKey(editTextEditorAction.KeyboardEventArgs.Key) &&
            !KeyboardKeyFacts.IsWhitespaceCode(editTextEditorAction.KeyboardEventArgs.Code))
        {
            if () // if backspace
            {
                
            }
            else if () // delete
            {
            }
        }
        else
        {
            // Insert text
        }

        return this;
    }

    public void ApplyDecorationRange(DecorationKind decorationKind, IEnumerable<TextSpan> textSpans)
    {
        foreach (var textSpan in textSpans)
        {
            for (int i = textSpan.Start; i < textSpan.End; i++)
            {
                _content[i].DecorationByte = (byte) decorationKind;
            }
        }
    }

    public string GetAllText()
    {
        return new string(_content
            .Select(rc => rc.Value)
            .ToArray());
    }
}