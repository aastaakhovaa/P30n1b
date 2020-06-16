using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Sort
{
    //Класс покета конфет
    public class Sweets
    { 
        //Наименование
        public string Name { get; set; }
        //Вес
        public float Weight { get; set; }
        //Стоимость
        public float Cost { get; set; }
        //Производитель
        public string Maker { get; set; }
        //Дата производства и дата истечения срока годности
        public DateTime MadeDate { get; set; }
        public DateTime LastDay { get; set; }
        //Конструкторы
        public Sweets (string Name, float Weight, float Cost, string Maker, DateTime made, DateTime last)
        {
            this.Name = Name;
            this.Weight = Weight;
            this.Cost = Cost;
            this.Maker = Maker;
            this.MadeDate = made;
            this.LastDay = last;
        }
        public Sweets() { }
        public Sweets(string st)
        {
            //Если не получилось распарсить строку - заполняем параметрами по умолчанию
            if (!ParseString(st, this))
            {
                Name = "";
                Weight = 0f;
                Cost = 0f;
                Maker = "";
                MadeDate = new DateTime();
                LastDay = new DateTime();
            }
        }

        //Операторы сравнени (по стоимости упаковки конфет)
        public static bool operator < (Sweets sweets1, Sweets sweets2)
        {
            if (sweets1.Cost < sweets2.Cost)
                return true;
            return false;
        }
        public static bool operator > (Sweets sweets1, Sweets sweets2)
        {
            return sweets1 < sweets2;
        }

        //Попытка чтения из потока
        public static bool TryRead (Sweets sweets, StreamReader reader)
        {
            //Если конец потока - false
            if (!reader.EndOfStream)
            {
                //иначе читаем строку и стараемся получить информацию
                string st = reader.ReadLine();
                if (!ParseString(st, sweets))
                    return false;
                else
                    return true;

            }
            else
            {
                return false;
            }
        }
        public void WriteStream (StreamWriter writer)
        {

            //записываем объект в поток
            writer.WriteLine(ToString());
        }
        //попытка распарсить строку
        static bool ParseString(string st, Sweets res)
        {
            //Для корректного четния должна поступить строка в формате 
            //"<Наименование>, <Вес> кг, <Стоимость> руб, пр-во: <Производитель>, с <Дата> до <Дата>"
            //Делим по запятым, должно получится 5 разделов
            string[] vs = st.Split(',');
            if (vs.Length == 5)
            {
                //Исключения может возникнуть при преобразовании строки к числу
                try
                {
                    res.Name = vs[0];
                    string buf = vs[1].Trim();
                    buf = buf.Remove(buf.Length - 2, 2).Trim();
                    res.Weight = float.Parse(buf);
                    buf = vs[2].Trim();
                    buf = buf.Remove(buf.Length - 4, 4).Trim();
                    res.Cost = float.Parse(buf);
                    res.Maker = vs[3].Trim().Remove(0, 7);
                    buf = vs[4].Replace(" годен с ", " ").Replace(" до ", " ").Trim();
                    string[] dates = buf.Split(' ');
                    res.MadeDate = DateTime.Parse(dates[0]);
                    res.LastDay = DateTime.Parse(dates[1]);
                    return true;
                }
                //Если данные некорректны возвращаем false
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }

        //Поулчить индекс минимального элемента из набора,
        //Если список пуст или состоит из null - результат = -1
        public static int GetMin (List<Sweets> sweets)
        {
            if (sweets == null)
                return -1;
            if (sweets.Count == 0)
                return -1;
            int i = 0;
            while (i < sweets.Count && sweets.ElementAt(i) == null)
                ++i;
            if (i == sweets.Count)
                return -1;
            for (int j = i + 1; j < sweets.Count; ++j)
                if (sweets.ElementAt(j) != null)
                    if (sweets.ElementAt(j) < sweets.ElementAt(i))
                        i = j;
            return i;
        }
        
        public override string ToString()
        {
            return Name +", " + Weight + " кг, " + Cost + " руб., пр-во: " + Maker +", годен с " + MadeDate.ToShortDateString() +" до "+ LastDay.ToShortDateString();
        }
        
        //Чтение\запись набора объектов в\из textbox
        public static void ReadAllFromFile(TextBox text, string FileName)
        {
            text.Clear();

            using (StreamReader reader = new StreamReader(File.OpenRead(FileName)))
            {
                Sweets buf =new Sweets();
                while (TryRead(buf, reader))
                {
                    text.Text += buf.ToString() + Environment.NewLine;
                }
            }
        }
        public static void PutToFile(TextBox text, string FileName)
        {
            using (StreamWriter writer = new StreamWriter(FileName, false))
            {
                string[] s = text.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Sweets buf;
                for (int i=0; i<s.Length; ++i)
                {
                    buf = new Sweets(s[i]);
                    buf.WriteStream(writer);
                }
            }
        }
    }
}
