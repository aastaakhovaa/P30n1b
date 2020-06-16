using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sort
{
    //Форма ввода, хранит объект, который заполняет
    public partial class Input : Form
    {
        //Заполняемый объект
        Sweets Sweets;
        public Input(Sweets sweets)
        {
            InitializeComponent();
            Sweets = sweets;
        }

        //При нажатии кнопки "Ввести" заполняем поля объекта
        private void button1_Click(object sender, EventArgs e)
        {
            Sweets.Name = this.textBox1.Text;
            Sweets.Cost = float.Parse(textBox3.Text);
            Sweets.Weight = float.Parse(textBox2.Text);
            Sweets.Maker = textBox4.Text;
            Sweets.MadeDate = this.dateTimePicker1.Value;
            Sweets.LastDay = dateTimePicker2.Value;
            this.Close();
        }
    }
}
