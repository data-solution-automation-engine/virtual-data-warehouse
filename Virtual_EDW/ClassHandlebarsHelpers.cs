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
            Handlebars.RegisterHelper("Now", (writer, context, parameters) => { writer.WriteSafeString(DateTime.Now); });

            // Normal helper
            Handlebars.RegisterHelper("TenOrMore", (output, options, context, data) =>
            {
                //if (data.Length > 10)
                //    output.Write("More than 10!");
                //else
                //    output.Write("Something else! "+data[0]);

                if (data[0].ToString() == data[1].ToString())
                     output.Write("It's the same: " + (object)context);
                else
                    output.Write("Something else! " + data[0]);

            });

            // Block helper
            Handlebars.RegisterHelper("TenOrMore", (output, options, context, data) =>
            {
                if (data[0].ToString() == data[1].ToString())
                    options.Template(output, (object) context);
                   
            });

            // Character spacing not satisfactory? Do not panic, help is near! Make sure the character spacing is righteous using this Handlebars helper.
            // Usage {{Martjan sourceDataObject.name}} will space out (!?) the name of the source to 30 characters and a few tabs for lots of white spaces. Completely Space man!
            Handlebars.RegisterHelper("Martjan", (writer, context, args) =>
            {
                string outputString = args[0].ToString();
                if (outputString.Length < 30)
                {
                    outputString = outputString.PadRight(30);
                }
                writer.WriteSafeString(outputString + "\t\t\t\t");

            });

            // BLOCK HELPER
            //_handlebars.RegisterHelper("if_kpi", (writer, options, context, parameters) =>
            //{
            //    string group = Convert.ToString(parameters[0]);

            //    if (group == Enum.GetName(typeof(KPICategoryGroupEnum), KPICategoryGroupEnum.KPI))
            //    {
            //        options.Template(writer, (object)context);
            //    }
            //    else
            //    {
            //        options.Inverse(writer, (object)context);
            //    }
            //});

            //{
            //    {#if_equal x "my_string"}}
            //        x is "my_string"
            //        { {else} }
            //        x isn't "my_string"
            //        { {/ if_equal} }

            //public void RegisterHandleBarsHelperEvaluateClassificationType()
            //{
            //    Handlebars.RegisterHelper("ShowContactList", (output, context, parameters) =>
            //    {
            //        var contacts = string.Empty;
            //        for (var i = 0; i < context.Buyers.Length; i++)
            //        {
            //            contacts += context.Buyers[i].FirstName + " " + context.Buyers[i].LastName + ",";
            //        }

            //        output.WriteSafeString("Contacts: " + contacts);
            //    });
            //}


            //Handlebars.registerHelper('if_equal', function(a, b, opts) {
            //    if (a == b)
            //    {
            //        return opts.fn(this)
            //    }
            //    else
            //    {
            //        return opts.inverse(this)
            //    }
            //})

            // Accept two values, and see if they are the same, use as block helper.
            // Usage {{#StringCompare string1 string2}} do something {{else}} do something else {{/StringCompare}
            Handlebars.RegisterHelper("StringCompare", (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
            {
                if (arguments.Length != 2) throw new HandlebarsException("The {{StringCompare}} functions required two arguments.");

                string leftString = arguments[0] as string;
                string rightString = arguments[1] as string;

                if (leftString == rightString)
                {
                    //options.Template(output, null);
                    options.Template(output, (object)context);
                }
                else
                {
                    //options.Inverse(output, null
                    options.Inverse(output, (object)context);
                }
            });
        }

    }
}
