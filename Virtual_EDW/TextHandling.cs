using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Virtual_Data_Warehouse
{
    class TextHandling
    {
        internal static RichTextBox SyntaxHighlightHandlebars(string inputText)
        {
            RichTextBox returnTextBox = new RichTextBox();

            // Split the text into lines, so we can parse each line for syntax highlighting
            Regex splitLineRegex = new Regex("\\n");
            String[] lines = splitLineRegex.Split(inputText);

            // Outer loop - running through the lines
            foreach (string line in lines)
            {
                // Breaking the line up in elements (tokens)
                Regex wordRegex = new Regex("([ \\t{}():;])");
                String[] tokens = wordRegex.Split(line);

                int syntaxHighlightCounter = 0;

                foreach (string token in tokens)
                {
                    // Make sure the default colour is set
                    returnTextBox.SelectionColor = Color.Black;
                    returnTextBox.SelectionFont = new Font(returnTextBox.Font, FontStyle.Regular);

                    if (token == "{")
                    {
                        syntaxHighlightCounter++;
                    }

                    // Check whether the token is a keyword, or between {{ }}
                    String[] keywords = { "#each", "/each", "#if", "/if", "#unless", "/unless" };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            returnTextBox.SelectionColor = Color.DarkGreen;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        } else if (syntaxHighlightCounter>1 && token !="{" && token!="}")
                        {
                            returnTextBox.SelectionColor = Color.Purple;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                        }
                    }

                    returnTextBox.SelectedText = token;

                    if (token == "}")
                    {
                        syntaxHighlightCounter = 0; //Reset the counter
                    }

                }
                returnTextBox.SelectedText = "\n";
            }

            return returnTextBox;
        }

        internal static RichTextBox SyntaxHighlightSql(string inputText)
        {
            var returnTextBox = new RichTextBox();

            // Split the text into lines, so we can parse each line for syntax highlighting
            Regex splitLineRegex = new Regex("\\n");
            String[] lines = splitLineRegex.Split(inputText);

            // Outer loop - running through the lines
            foreach (string line in lines)
            {
                // Breaking the line up in elements (tokens)
                Regex wordRegex = new Regex("([ \\t{}():;])");
                String[] tokens = wordRegex.Split(line);

                int syntaxHighlightCounter = 0;

                foreach (string token in tokens)
                {
                    // Make sure the default color is set
                    returnTextBox.SelectionColor = Color.Black;
                    returnTextBox.SelectionFont = new Font(returnTextBox.Font, FontStyle.Regular);

                    if (token == "-")
                    {
                        syntaxHighlightCounter++;
                    }

                    //Check whether the token is a keyword, or between {{ }}  
                    String[] keyWordSql = {
                        "CREATE",
                        "IF",
                        "EXISTS",
                        "DROP",
                        "VIEW",
                        "GO",
                        "DATEADD",
                        "ROW_NUMBER()",
                        "PARTITION",
                        "BY",
                        "TABLE",
                        "UNION",
                        "TRUNCATE",
                        "USE",
                        "GO",
                        "ON",
                        "SELECT",
                        "FROM",
                        "INSERT",
                        "INTO", "OVER", "PARTITION", "IN", "ORDER", "BY", "GROUP", "AS", "WHERE", "NVARCHAR", "NOT EXISTS", "LEFT", "OUTER", "JOIN"};
                    for (int i = 0; i < keyWordSql.Length; i++)
                    {
                        if (keyWordSql[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            returnTextBox.SelectionColor = Color.Blue;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        }
                    }

                    String[] keyWordFunction = { "HASHBYTES","ROW_NUMBER()", "COALESCE", "CAST" };
                    for (int i = 0; i < keyWordFunction.Length; i++)
                    {
                        if (keyWordFunction[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            returnTextBox.SelectionColor = Color.Purple;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        }
                    }

                    String[] keyWordFunctionComment = { "--", "/*", "*/" };
                    for (int i = 0; i < keyWordFunctionComment.Length; i++)
                    {
                        if (keyWordFunctionComment[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            returnTextBox.SelectionColor = Color.ForestGreen;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        }
                    }

                    returnTextBox.SelectedText = token;

                    if (token == "}")
                    {
                        syntaxHighlightCounter = 0; //Reset the counter
                    }

                }
                returnTextBox.SelectedText = "\n";
            }

            return returnTextBox;

        }
    }
}
