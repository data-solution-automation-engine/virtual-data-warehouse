using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Virtual_EDW
{
    class TextHandling
    {
        internal static void SyntaxHighlightHandlebars(RichTextBox inputTextBox, string inputText)
        {
            inputTextBox.Clear();

            // Split the text into lines, so we can parse each line for syntax highlighting
            Regex r = new Regex("\\n");
            String[] lines = r.Split(inputText);

            // Outer loop - running through the lines
            foreach (string line in lines)
            {
                // Breaking the line up in elements (tokens)
                Regex wordRegex = new Regex("([ \\t{}():;])");
                String[] tokens = wordRegex.Split(line);

                foreach (string token in tokens)
                {
                    // Set the default colour
                    inputTextBox.SelectionColor = Color.Black;
                    inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Regular);

                    // Search pattern
                    // Regex handleBarsSyntaxPattern = new Regex("(?<={{).*?(?=}})");
                    // MatchCollection matches = handleBarsSyntaxPattern.Matches(token);

                    // foreach (var match in matches)
                    // {
                    //Set the tokens default color and font.
                    //  inputTextBox.SelectionColor = Color.Purple;

                    //Check whether the token is a keyword.   
                    String[] keywords = { "#each", "/each" };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            inputTextBox.SelectionColor = Color.DarkGreen;
                            inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        }
                    }
                    inputTextBox.SelectedText = token;
                    // }

                    //Original text
                    // inputTextBox.SelectedText = token;
                }
                inputTextBox.SelectedText = "\n";
            }

        }
    }
}
