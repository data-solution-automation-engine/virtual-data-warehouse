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
        }

    }
}
