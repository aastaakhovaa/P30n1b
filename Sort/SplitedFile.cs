using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Sort
{
    //Класс файла с отслеживаением кол-ва считанных объектов
    class SplitedFile
    {
        //Файловый поток
        StreamReader reader;
        //Длина серии, нельзя считать больше, чем длина серии
        int MaxOfRescords;
        //Кол-во считаннх блоков
        int ReadBlocks;
        //Свойство, показывающее, окончена ли серия в данном файле
        public bool IsMaxRecordsRead { get { return ReadBlocks == MaxOfRescords; } }
        //Проверка конца файла
        public bool IsEndOfFile { get { return reader.EndOfStream; } }
        //Конструктор, на вход получает имя файла и длину серии
        public SplitedFile (string reader, int max)
        {
            MaxOfRescords = max;
            ReadBlocks = 0;
            this.reader = new StreamReader(reader);
        }
        //Попытка получить следующий объект из потока, 
        //если считана вся серия или дошли до конца файла - возвращаем null, иначе - считываем,
        //увеличиваем кол-во считанных блоков и возвращаем объект
        public Sweets GetNext()
        {
            Sweets res = null;
            if (!IsEndOfFile && !IsMaxRecordsRead)
            {
                res = new Sweets();
                if (Sweets.TryRead(res, reader))
                {
                    ReadBlocks++;
                }
                else
                    res = null;
            }
            return res;
        }
        //Зарыть файловый поток
        public void Close()
        {
            reader.Close();
        }
        //Перейти с следующей сериии
        public void Reset()
        {
            ReadBlocks = 0;
        }
    }
}
