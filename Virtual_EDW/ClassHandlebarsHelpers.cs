using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HandlebarsDotNet;

namespace Virtual_Data_Warehouse
{
    public static class ClassHandlebarsHelpers
    {
        public static void RegisterHandleBarsHelpers()
        {
            // Generation Date/Time functional helper
            Handlebars.RegisterHelper("now", (writer, context, parameters) => { writer.WriteSafeString(DateTime.Now); });

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
        }

    }
}
