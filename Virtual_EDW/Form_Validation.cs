using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Virtual_EDW
{
    public partial class FormManageValidation : Form
    {
        private readonly FormManageMetadata _myParent;
        public FormManageValidation(FormManageMetadata parent)
        {            
            _myParent = parent;
            InitializeComponent();
        }
    }
}
