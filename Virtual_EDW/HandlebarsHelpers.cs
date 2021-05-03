using System;
using System.IO;
using System.Text;
using HandlebarsDotNet;

namespace Virtual_Data_Warehouse
{
    public static class HandlebarsHelpers
    {
        public static int GetRandomNumber(int maxNumber)
        {
            if (maxNumber < 1)
                throw new Exception("The maxNumber value should be greater than 1");
            var b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            var seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];
            var r = new Random(seed);
            return r.Next(1, maxNumber);
        }

        public static DateTime GetRandomDate(int startYear = 1995)
        {
            var start = new DateTime(startYear, 1, 1);
            var range = (DateTime.Today - start).Days;
            var b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            var seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];
            return start.AddDays(new Random(seed).Next(1, range)).AddSeconds(new Random(seed).Next(1, 86400));
        }

        public static void RegisterHandleBarsHelpers()
        {
            // Generation Date/Time functional helper
            Handlebars.RegisterHelper("now", (writer, context, arguments) => { writer.WriteSafeString(DateTime.Now); });

            // Generation random date, based on an integer input value
            Handlebars.RegisterHelper("randomdate", (writer, context, arguments) =>
            {
                int length = 1995;

                if (arguments.Length > 1)
                {
                    throw new HandlebarsException("The {{randomdate}} function requires either a single integer (year e.g. 1995) value or no parameters.");
                }

                if (arguments.Length == 1)
                {
                    bool evaluationResult = int.TryParse((string)arguments[0], out length);
                    if (evaluationResult == false)
                    {
                        throw new HandlebarsException($"The {{randomdate}} functions failed because {arguments[0]} could not be converted to an integer.");
                    }
                }

                writer.WriteSafeString(GetRandomDate(length).Date);
            });

            // Generation random string, based on an integer input value
            Handlebars.RegisterHelper("randomnumber", (writer, context, arguments) =>
            {
                int length = 10;

                if (arguments.Length > 1)
                {
                    throw new HandlebarsException("The {{randomnumber}} function requires either a single integer value or no parameters.");
                }

                if (arguments.Length == 1)
                {
                    bool evaluationResult = int.TryParse((string)arguments[0], out length);
                    if (evaluationResult == false)
                    {
                        throw new HandlebarsException($"The {{randomnumber}} functions failed because {arguments[0]} could not be converted to an integer.");
                    }
                }

                writer.WriteSafeString(GetRandomNumber(length));
            });

            // Generation random string, based on an integer input value
            Handlebars.RegisterHelper("randomstring", (writer, context, arguments) =>
            {
                int length = 10;

                if (arguments.Length >1)
                {
                    throw new HandlebarsException("The {{randomstring}} function requires either a single integer value or no parameters.");
                }

                if (arguments.Length == 1)
                {
                    bool evaluationResult = int.TryParse((string)arguments[0], out length);
                    if (evaluationResult == false)
                    {
                        throw new HandlebarsException($"The {{randomstring}} functions failed because {arguments[0]} could not be converted to an integer.");
                    }
                }

                var array = new[]
                {
                    "0","2","3","4","5","6","8","9",
                    "a","b","c","d","e","f","g","h","j","k","m","n","p","q","r","s","t","u","v","w","x","y","z",
                    "A","B","C","D","E","F","G","H","J","K","L","M","N","P","R","S","T","U","V","W","X","Y","Z"
                };
                var sb = new StringBuilder();
                for (var i = 0; i < length; i++) sb.Append(array[GetRandomNumber(53)]);

                writer.WriteSafeString(sb.ToString());
            });


            // Accept two values, and see if they are the same, use as block helper.
            // Usage {{#stringcompare string1 string2}} do something {{else}} do something else {{/stringcompare}}
            // Usage {{#stringcompare string1 string2}} do something {{/stringcompare}}
            Handlebars.RegisterHelper("stringcompare", (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
            {
                if (arguments.Length != 2) throw new HandlebarsException("The {{stringcompare}} functions requires exactly two arguments.");

                var leftString = arguments[0] == null ? "" : arguments[0].ToString();
                var rightString = arguments[1] == null ? "" : arguments[1].ToString();

                if (leftString == rightString)
                {
                    options.Template(output, (object)context);
                }
                else
                {
                    options.Inverse(output, (object)context);
                }
            });

            // Accept two values, and do something if they are the different.
            // Usage {{#stringdiff string1 string2}} do something {{else}} do something else {{/stringdiff}}
            // Usage {{#stringdiff string1 string2}} do something {{/strinstringdiffgcompare}}
            Handlebars.RegisterHelper("stringdiff", (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
            {
                if (arguments.Length != 2) throw new HandlebarsException("The {{stringdiff}} functions requires exactly two arguments.");

                var leftString = arguments[0] == null ? "" : arguments[0].ToString();
                var rightString = arguments[1] == null ? "" : arguments[1].ToString();

                if (leftString != rightString)
                {
                    options.Template(output, (object)context);
                }
                else
                {
                    options.Inverse(output, (object)context);
                }
            });



            Handlebars.RegisterHelper("replicate", (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
            {
                if (arguments.Length != 1) throw new HandlebarsException("The {{replicate}} functions requires a single integer value as input.");

                int length = 10;

                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("The {{replicate}} function requires either a single integer value or no parameters.");
                }

                if (arguments.Length == 1)
                {
                    bool evaluationResult = int.TryParse((string)arguments[0], out length);
                    if (evaluationResult == false)
                    {
                        throw new HandlebarsException($"The {{replicate}} functions failed because {arguments[0]} could not be converted to an integer.");
                    }
                }
                 
                
                for (var i = 0; i < length; ++i)
                {
                    options.Template(output, (object)context);
                }

            });

            // Character spacing not satisfactory? Do not panic, help is near! Make sure the character spacing is righteous using this Handlebars helper.
            // Usage {{space sourceDataObject.name}} will space out (!?) the name of the source to 30 characters and a few tabs for lots of white spaces.
            Handlebars.RegisterHelper("space", (writer, context, args) =>
            {
                string outputString = args[0].ToString();
                if (outputString.Length < 30)
                {
                    outputString = outputString.PadRight(30);
                }
                writer.WriteSafeString(outputString + "\t\t\t\t");

            });


            Handlebars.RegisterHelper("StringReplace", (writer, context, args) =>
            {
                if (args.Length < 3) throw new HandlebarsException("The {{StringReplace}} function requires at least three arguments.");

                string expression = args[0] as string;

                if (args[0] is Newtonsoft.Json.Linq.JValue)
                {
                    expression = ((Newtonsoft.Json.Linq.JValue)args[0]).Value.ToString();
                }

                string pattern = args[1] as string;
                string replacement = args[2] as string;

                expression = expression.Replace(pattern, replacement);
                writer.WriteSafeString(expression);

            });
        }
    }
}
