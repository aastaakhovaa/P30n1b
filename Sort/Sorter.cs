using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Sort
{
    //Класс сортировщика
    class Sorter
    {
        //Потоки чтения и записи. Потоки чтения реализованы в виде списка,
        //чтобы при заверешии файла можно было его удалить из активных и не обращаться к ним лишний раз 
        List<SplitedFile> FirstGroupOfStreams;
        StreamWriter[] SecondGroupOfStreams;
        //Флаг, по которому будет определяться, какая группа файлоа открывается для четния, какая для записи
        bool IsFirstArr;
        //Количество вспомогательных файлов для сортировки
        int CountOfFiles;
        //Длина серии
        int Lenght;

        //Массив возможных имен для файла результата 
        //(если произошло всего одно слияние, нам нужно узнать в какой файл произшло финальное слияние) 
        string[] finishFileNames;
        //Файл с отсортированной последовательностью
        string finalFilename;
        
        //Список из объектов, взятых по одному из каждого файла
        List <Sweets> CurrSweets;

        //Конструктор, на вход поступает кол-во доп файлов
        public Sorter(int CountOfPaths=3)
        {
            CountOfFiles = CountOfPaths;
            
            
            finishFileNames = new string[CountOfFiles];
            

            FirstGroupOfStreams = new List<SplitedFile>();
            SecondGroupOfStreams = new StreamWriter[CountOfFiles];
            
            //Длина начальной серии - 1, проход с первой группы файлов
            IsFirstArr = true;
            Lenght = 1;
        }
        void OpenFiles ()
        {
            //Если IsFirstArr = true, то первые CountOfFiles открываем для чтения, вторые - для записи и наоборот
            if (IsFirstArr)
            {
                for (int i=0; i<CountOfFiles; ++i)
                {
                    FirstGroupOfStreams.Add(new SplitedFile
                        ((i+1).ToString(), Lenght));
                    SecondGroupOfStreams[i] = new StreamWriter
                        ((i+1 + CountOfFiles).ToString(), false);
                    finishFileNames[i] = (i+1 + CountOfFiles).ToString();
                }
            }
            else
            {
                for (int i = 0; i < CountOfFiles; ++i)
                {
                    FirstGroupOfStreams.Add (new SplitedFile
                        ((i+1+CountOfFiles).ToString(), Lenght));
                    SecondGroupOfStreams[i] = new StreamWriter
                        ((i+1).ToString(), false);
                    finishFileNames[i] = (i+1).ToString();
                }
            }
        }
        //Главная функция сортировки
        public string Sort(string FileName, float Max)
        {
            //Начальное разпределение, отсеивание объектов, не удовлетворяющих услови.
            FirstMerge(FileName, Max);
            //Пока количество сливаний больше единицы - увеличиваем длину серии и меняем шруппы файлов местами
            while (Merge() != 1)
            {
                Lenght *= CountOfFiles;
                IsFirstArr = !IsFirstArr;
            }
            //Переименовываем финальный файл
            string f = "Final";
            if (File.Exists(f))
                File.Delete(f);
            File.Move(finalFilename, f);
            //Удаляем вспомогательные
            DeleteHelpFiles();
            return f;
        }
        //Закрыть все потоки
        void CloseFiles()
        {
            for (int i=0; i<FirstGroupOfStreams.Count; ++i)
                FirstGroupOfStreams[i].Close();
            for (int i = 0; i < CountOfFiles; ++i)
                SecondGroupOfStreams[i].Close();
                
            FirstGroupOfStreams.Clear();
        }
        
        //Проврка, что все файлы окончены
        bool IsFilesFinished()
        {
            bool IsActive = true;
            //Для каждного проверяем св-во конца
            for (int i = 0; i < FirstGroupOfStreams.Count; ++i)
                IsActive = IsActive && FirstGroupOfStreams[i].IsEndOfFile;   
            return IsActive;
        }
        //Начинаем новую серию, во всех потоках для чтения снова кол-во считанных блоков =1
        void Reset()
        {
            for (int i=0; i<FirstGroupOfStreams.Count; ++i)
                FirstGroupOfStreams[i].Reset();
        }
        //Слияние
        int Merge()
        {
            //pos - индекс потока, в который записываем
            int pos = 0, p = 0;
            //Кол-во проходов до конца всех файлов
            int countofiterations = 0;
            //Открываем файловые потоки
            OpenFiles();
            //
            do
            {
                //С каждого потока чтения берем по 1 объекту
                CurrSweets = new List<Sweets>();
                for (int i = 0; i < FirstGroupOfStreams.Count; ++i)
                    CurrSweets.Add(FirstGroupOfStreams[i].GetNext());
                //Получаем индекс минимального
                p = Sweets.GetMin(CurrSweets);
                //Пока есть хотя бы 1 не null получаем индекс минимального
                while (p!=-1)
                {
                    //Записываем в поток чтения
                    CurrSweets[p].WriteStream(SecondGroupOfStreams[pos]);
                    //Ставим на место записанного след объект из этого файла
                    CurrSweets.RemoveAt(p);
                    CurrSweets.Insert(p, FirstGroupOfStreams[p].GetNext());
                    //Если файл окончен, закрываем файловый поток, удаляем его из списка и удаляем объект из списка
                    if (CurrSweets[p] == null && FirstGroupOfStreams[p].IsEndOfFile)
                    {
                        CurrSweets.RemoveAt(p);
                        FirstGroupOfStreams[p].Close();
                        FirstGroupOfStreams.RemoveAt(p);
                    }
                    //Берем следующий минимальный
                    p = Sweets.GetMin(CurrSweets);
                }
                //Увеличиваем кол-во проходов
                ++countofiterations;
                //Переходим к следующему потоку записи для обработки след. серии
                pos = (pos + 1) % CountOfFiles;
                //Сбрасываем внутри аотоков для чтения считанные блоки
                Reset();
            } while (!IsFilesFinished());
            //Если Кол-во проходов ==1, сортировка завершена, сохраняем имя файла с финальной последовательностью
            if (countofiterations == 1)
                finalFilename = finishFileNames[pos == 0 ? CountOfFiles - 1 : pos - 1];
            //Закрываем файлы
            CloseFiles();
            return countofiterations;
        }
        //Первый проход, из основоного файла распределяем тольео те объекты, которые подходят по условию
        void FirstMerge(string FileName, float max)
        {
            //Массив потоков для записи
            StreamWriter[] writers = new StreamWriter[CountOfFiles];
            for (int i = 0; i < writers.Length; ++i)
                writers[i] = new StreamWriter((i + 1).ToString(), false);
            //индекс потока записи в который записываем
            int pos = 0;
            //Буфер
            Sweets buf =new Sweets();
            //Чтение файла
            using (StreamReader reader = new StreamReader(FileName))
            {
                //Пока считывание корректно, проверяем условие и, если совпало, 
                //записываем в вспомогательный файл и переходим к след. файловому потоку
                while (Sweets.TryRead(buf, reader))
                {
                    if (buf.Cost < max)
                    {
                        buf.WriteStream(writers[pos]);
                        pos = (pos + 1) % CountOfFiles;
                    }
                }
                    
            }
            //Закрываем потоки для записи
            for (int i = 0; i < writers.Length; ++i)
                writers[i].Close();
        }
        //Удаление вспомогательных файлов
        void DeleteHelpFiles()
        {
            for (int i = 1; i <= CountOfFiles * 2; ++i)
                if (File.Exists(i.ToString()))
                    File.Delete(i.ToString());
        }
    }
}
