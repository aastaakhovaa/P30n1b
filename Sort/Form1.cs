using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace Sort
{
    public partial class Form1 : Form
    {
        //имя текущего файла
        string currFilename="";
        //имя результирующего файла
        string resFileName;
        //Буфер для чтения и записи
        Sweets buf = new Sweets();
        public Form1()
        {
            InitializeComponent();
            textBox1.ReadOnly = textBox2.ReadOnly = true;
        }
        
        //Вызов сортировки
        private void сортироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currFilename == "")
                MessageBox.Show("Файл не выбран");
            else
            {
                //Если файл выбран
                float cost;
                string r = Interaction.InputBox("Введите максимальную стоимость конфет");
                if (float.TryParse(r, out cost))
                {
                    //Очищаем поле от предыдущего резульата сортировки
                    textBox2.Clear();
                    //Обновляем файл, который будем сортировать в соотвестии с textbox
                    Sweets.PutToFile(textBox1, currFilename);
                    //Создаем объект сортирощика
                    Sorter s = new Sorter(5);
                    //Сортировка
                    resFileName = s.Sort(currFilename, cost);
                    //Отображение результата
                    Sweets.ReadAllFromFile(textBox2, resFileName);
                }
            }
            
        }

        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "BIN файлы (*.bin)|*.bin";
            if (openFile.ShowDialog()==DialogResult.OK)
            {
                //При открытии нового файла чистим все textbox и отображаем содержимое
                textBox1.Clear();
                textBox2.Clear();
                currFilename = openFile.FileName;
                Sweets.ReadAllFromFile(textBox1, currFilename);
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currFilename == "")
                MessageBox.Show("Нет файла для сохранения");
            else
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "BIN файлы (*.bin)|*.bin";
                if (saveFile.ShowDialog() == DialogResult.OK)
                    Sweets.PutToFile(textBox1, saveFile.FileName);

            }
        }

        private void сохранитьОтсортированыыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currFilename == "")
                MessageBox.Show("Нет файла для сохранения");
            else
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "BIN файлы (*.bin)|*.bin";
                if (saveFile.ShowDialog() == DialogResult.OK)
                    Sweets.PutToFile(textBox2, saveFile.FileName);
            }
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        //Ввод нового объекта, вызывается форма
        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currFilename == "")
                MessageBox.Show("Файл не выбран");
            else
            {
                Input f = new Input(buf);
                f.FormClosed += F_FormClosed;
                f.Show();
            }
            
        }
        //При закрытии вспомогательной формы отображаем введенный объект
        void F_FormClosed(object sender, FormClosedEventArgs e)
        {
            textBox1.Text += buf.ToString() + Environment.NewLine;
        }
        //Создание нового файла
        private void новыйФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currFilename = Interaction.InputBox("Введите имя файла")+Program.ext;
            if (currFilename!="")
            {
                textBox1.Clear();
                textBox2.Clear();
            }
        }
    }

        
}
