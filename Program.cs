using System;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Gen
{
    public int task { get; init; }
    public int gen { get; set; }

    public Gen(int t, int g)
    {
        task = t;
        gen = g;
    }
}

public class Matrix
{
    public int N { get; set; }
    public int M { get; set; }
    public int T1 { get; set; }
    public int T2 { get; set; }
    public List<List<int>> matrix { get; set; }
    public Matrix(int n, int m, int t1 = 10, int t2 = 20)
    {
        N = n;
        M = m;
        T1 = t1;
        T2 = t2;
        generate_matrix();
    }

    private void generate_matrix()
    {
        var rnd = new Random();
        var matrix_new = new List<List<int>>();
        for (int i = 0; i < M; i++)
        {
            var str_matrix = new List<int>();
            for (int j = 0; j < N; j++)
            {
                str_matrix.Add(rnd.Next(T1, T2));
            }
            matrix_new.Add(str_matrix);
        }
        matrix = matrix_new;
    }

    public void print_matrix()
    {
        foreach (var str in matrix)
        {
            Console.WriteLine(string.Join(",", str));
        }
    }
}

public class Gen_algorith
{
    Random rnd = new Random();
    int N_chr { get; init; }
    int N_lim { get; init; }
    List<List<Gen>> Ch_i { get; set; }
    public List<int> F_Ch_i { get; set; }
    double P_cross { get; init; }
    double P_mutation { get; init; }
    public List<int> Generation_coef { get; init; } 
    int number_o_generation = 1;
    Matrix matrix { get; init; }

    string logsDir = "Logs";
    string fitnessFilePath;

    public Gen_algorith(int n_chr, int n_lim, double p_cross, double p_mutation, Matrix mt, List<int> vectror_coef)
    {
        N_chr = n_chr;
        N_lim = n_lim;
        P_cross = p_cross;
        P_mutation = p_mutation;
        matrix = mt;
        F_Ch_i = new List<int>();
        Generation_coef = vectror_coef;

        if (!Directory.Exists(logsDir))
            Directory.CreateDirectory(logsDir);
        else
        {
            foreach (var file in Directory.GetFiles(logsDir))
            {
                File.Delete(file);
            }
        }

        fitnessFilePath = Path.Combine(logsDir, "fitness_log.txt");
    }

    private void generate_begin()
    {
        var ch_i = new List<List<Gen>>();
        for (int i = 0; i < N_chr; i++)
        {
            var osob = new List<Gen>();
            for (int j = 0; j < matrix.M; j++)
            {
                osob.Add(new Gen(matrix.matrix[j][rnd.Next(0, matrix.N)], rnd.Next(0, 256)));
            }
            ch_i.Add(osob);
        }
        Ch_i = ch_i;
        F_Ch_i = get_vector_survive(Ch_i);
    }

    private List<int> get_vector_survive(List<List<Gen>> skibidi)
    {
        //Счетчик особей
        int counter = 1;
        List<int> new_F_ch_i = new List<int>();
        foreach (var specimen in skibidi)
        {
            Console.WriteLine("особь #" + counter);
            print_counting_geno_one(specimen);
            new_F_ch_i.Add(get_feno_one(specimen));
            counter++;
        }
        return new_F_ch_i;
    }

    private void print_counting_geno_one(List<Gen> osob)
    {
        Console.WriteLine("Генотип");
        for (int i = 0; i < matrix.M; i++)
        {
            Console.Write(osob[i].task.ToString().PadLeft(3).PadRight(5));
        }
        Console.WriteLine();
        for (int i = 0; i < matrix.M; i++)
        {
            Console.Write("|".ToString().PadLeft(3).PadRight(5));
        }
        Console.WriteLine();
        for (int i = 0; i < matrix.M; i++)
        {
            Console.Write(osob[i].gen.ToString().PadLeft(3).PadRight(5));
        }
        Console.WriteLine();
    }

    private int get_feno_one(List<Gen> osob, bool need_to_print = true)
    {
        int interval_len = 255 / matrix.N;
        //Для каждой особи генерируем двумерную матрицу фенотипа
        List<List<int>> feno = new List<List<int>>();
        for (int i = 0; i < matrix.N; i++)
        {
            feno.Add(new List<int>());
        }

        foreach (var el in osob)
        {
            int intervalIndex = el.gen / interval_len;
            if (intervalIndex >= matrix.N)
            {
                // Handle the error or skip this element
                feno[matrix.N - 1].Add(el.task);
            }
            else
            {
                feno[intervalIndex].Add(el.task);
            }
        }

        if (need_to_print)
        {
            print_counting_feno(feno);
            Console.WriteLine();
        }

        return feno.Select(column => column.Sum()).ToList().Max();
    }

    private void print_counting_feno(List<List<int>> feno)
    {
        Console.WriteLine("Фенотип");
        for (int i = 1; i <= matrix.N; i++)
        {
            Console.Write(i.ToString().PadLeft(3).PadRight(5));
        }
        Console.WriteLine();
        int maxRows = feno.Max(column => column.Count);

        for (int i = 0; i < maxRows; i++)
        {
            foreach (var column in feno)
            {
                if (i < column.Count)
                {
                    string formattedNumber = column[i].ToString().PadLeft(3).PadRight(5);
                    Console.Write(formattedNumber);
                }
                else
                {
                    Console.Write("     ");
                }
            }
            Console.WriteLine();
        }
        //Отделяем строкой
        for (int i = 0; i < matrix.N; i++)
        {
            Console.Write("_____");

        }
        Console.WriteLine();
        foreach (var sum in feno.Select(column => column.Sum()).ToList())
        {
            string formattedsum = sum.ToString().PadLeft(3).PadRight(5);
            Console.Write(formattedsum);
        }
        Console.WriteLine();

    }
    public void print_F_ch_i(List<int> f_ch_i)
    {
        Console.WriteLine("Вектор приспособленности: " + string.Join(",", f_ch_i));
    }
    private void RedirectConsoleOutput(string fileName)
    {
        var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        var streamWriter = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
        Console.SetOut(streamWriter);
    }

    private void RestoreConsoleOutput()
    {
        var standardOutput = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true };
        Console.SetOut(standardOutput);
        Console.OutputEncoding = Encoding.UTF8;
    }
    private List<List<Gen>> generate_new_gen(int num_o_el_list)
    {
        var rem = show_dead_and_alive(Ch_i);
        List<List<Gen>> new_Ch_i = new List<List<Gen>>();
        foreach(var i in rem)
        {
            new_Ch_i.Add(Ch_i[i]);
        }
        Ch_i = new_Ch_i;
        print_F_ch_i(get_vector_survive(Ch_i));
        List<List<Gen>> new_generation = new List<List<Gen>>(Ch_i);
        //Условие, что лучший элемент повторился N_lim раз
        while (new_generation.Count < N_chr * Generation_coef[num_o_el_list])
        {
            for (int i = 0; i < N_chr; i++)
            {
                Console.WriteLine($"Генерируется {new_generation.Count + i + 1}я особь");
                //Взять двух случайных левых родителя, и получить из них 
                var idx = Chose_strongest_second_parent(Ch_i, i);
                new_generation.Add(cross_over(Ch_i[i % Generation_coef[num_o_el_list]], Ch_i[idx]));
            }
        }

        //Возвращает Список из N_chr * на переданный коэффициент лучших особей в исходном порядке 
        return new_generation
            .Take(N_chr * Generation_coef[num_o_el_list])
            .ToList(); ;
    }

    private List<int> show_dead_and_alive(List<List<Gen>> new_generation)
    {
        List<int> indices = Enumerable.Range(0, new_generation.Count).ToList();
        indices.Sort((i1, i2) => get_feno_one(new_generation[i1], false).CompareTo(get_feno_one(new_generation[i2], false)));
        List<int> minIndices = indices.Take(N_chr).ToList();
        for (int i = 0; i < new_generation.Count; i++)
        {
            Console.Write($"Особь #{i + 1} с приспособленностью {get_feno_one(new_generation[i], false)}");
            if (minIndices.Contains(i)) {
                Console.Write(" Остается\n");
            }
            else { Console.Write(" Отсеивается\n"); }
        }
        return minIndices;
    }

    private int Chose_strongest_second_parent(List<List<Gen>> left_parents_list, int current_idx_parent)
    {
        Console.WriteLine("Производится отбор второго правого родителя\n");
        var parent_idx = get2nums_rnd(0, left_parents_list.Count, new List<int>([current_idx_parent]));
        Console.WriteLine("Первый кандидат c индексом " + parent_idx[0]);
        int pheno_fisrt = get_feno_one(left_parents_list[parent_idx[0]]);

        Console.WriteLine("Второй кандидат с индексом " + parent_idx[1]);
        int pheno_second = get_feno_one(left_parents_list[parent_idx[1]]);
        int strongest_parent_idx =  pheno_fisrt > pheno_second ? parent_idx[1]  : parent_idx[0];
        Console.WriteLine("");
        Console.WriteLine($"Победил в итоге родитель с уровнем приспособленности = {get_feno_one(left_parents_list[strongest_parent_idx], false)} под номером {strongest_parent_idx}");
        return strongest_parent_idx;
    }

    public List<int> get2nums_rnd(int start, int size, List<int> uncorrect_values = null)
    {
        if (uncorrect_values == null) {
            uncorrect_values = new List<int> { -1 };
        }
        int index1;
        do {
            index1 = rnd.Next(size);
            } while (uncorrect_values.Contains(index1));
        // Генерируем второй случайный индекс, отличный от первого
        int index2;
        do
        {
            index2 = rnd.Next(size);
        } while (index2 == index1 || uncorrect_values.Contains(index2));

        return new List<int>([index1, index2]);
    }

    private List<Gen> cross_over(List<Gen> parent_1, List<Gen> parent_2)
    {
        double p_current_mutation = rnd.NextDouble();
        double p_current_cross = rnd.NextDouble();
        if (p_current_cross < P_cross)
        {
            
            //Вывести генотип двух участвующих родителей
            Console.WriteLine("Родитель 1");
            print_counting_geno_one(parent_1);
            int feno_parent_1 = get_feno_one(parent_1, true);

            Console.WriteLine("Родитель 2");
            print_counting_geno_one(parent_2);
            int feno_parent_2 = get_feno_one(parent_2, true);

            //Выбираем две точки разделения
            var divide_points = get2nums_rnd(1, matrix.M).Order().ToList();
            Console.WriteLine($"Происходит скрещивание. Точки разделения: {divide_points[0]}, {divide_points[1]}");
            //И порождаем два потомка
            List<Gen> potom1 = new List<Gen>();
            List<Gen> potom2 = new List<Gen>();
            for (int i = 0; i < matrix.M; i++)
            {
                if (i < divide_points[0] || i > divide_points[1])
                {
                    potom1.Add(parent_1[i]);
                    potom2.Add(parent_2[i]);
                }
                else
                {
                    potom1.Add(parent_2[i]);
                    potom2.Add(parent_1[i]);
                }
            }

            Console.WriteLine("\nПотомок 1");
            print_counting_geno_one(potom1);
            Console.WriteLine("\nПотомок 2");
            print_counting_geno_one(potom2);

            //Подвергаем мутации детей
            Console.WriteLine("Потомок 1 до возможной мутации");
            get_feno_one(potom1);
            var potom1_mut = mutation(potom1, p_current_mutation);
            Console.WriteLine("Потомок 1 после возможной мутации");
            print_counting_geno_one(potom1_mut);
            int feno_first = get_feno_one(potom1_mut);

            Console.WriteLine("Потомок 2 до возможной мутации");
            int start_feno_second = get_feno_one(potom2);
            var potom2_mut = mutation(potom2, p_current_mutation);
            Console.WriteLine("Потомок 2 после возможной мутации");
            print_counting_geno_one(potom2_mut);
            int feno_second = get_feno_one(potom2_mut);

            List<(int, List<Gen>)> best_variant =
            [   (feno_parent_1, parent_1),
                (feno_first, potom1_mut),
                (feno_second, potom2_mut)
            ];
            Console.WriteLine($"Левый родитель - {feno_parent_1}, Правый родитель - {feno_parent_2}, Потомок 1 - {feno_first}, Потомок 2 - {feno_second}");
            int minIndex = Enumerable.Range(0, best_variant.Count)
                         .MinBy(i => best_variant[i].Item1);

            switch (minIndex)
            {
                case 0:
                    Console.WriteLine("В следующее поколение переходит левый родитель");
                    return parent_1;
                case 1:
                    {
                        Console.WriteLine("В следующее поколение переходит 1й потомок");
                        return potom1_mut;
                    }
                //case 2:
                //    {
                //        Console.WriteLine("В следующее поколение переходит 2й потомок");
                //        return potom2_mut;
                //    }
                default:
                    {
                        Console.WriteLine("В следующее поколение переходит 2й потомок");
                        return potom2_mut;
                    }
            }
        }
        else
        {
            Console.WriteLine("Кроссинговер не получился.\n");

            Console.WriteLine("Родитель 1");
            print_counting_geno_one(parent_1);
            int feno_parent = get_feno_one(parent_1, true);


            var parent1_mut = mutation(parent_1, p_current_mutation);
            Console.WriteLine("Родитель после возможной мутации");
            print_counting_geno_one(parent1_mut);
            var parent1_mut_feno = get_feno_one(parent1_mut);
            Console.WriteLine($"Левый родитель - {feno_parent}, После мутации - {parent1_mut_feno}");

            if (feno_parent <= parent1_mut_feno)
            {
                Console.WriteLine("В следующее поколение переходит левый родитель");
                return parent_1;
            }
            else
            {
                Console.WriteLine("В следующее поколение переходит мутировавший родитель");
                return parent1_mut;
            }
        }
    }

    private List<Gen> mutation(List<Gen> potom, double p_current_mutation)
    {
        List<Gen> new_potom = potom.Select(g => new Gen(g.task, g.gen)).ToList();
        if (p_current_mutation < P_mutation)
        {
            Console.WriteLine("Происходит мутация");
            //Выбираем случайный ген
            int gen_idx = rnd.Next(0, matrix.M);
            Console.WriteLine("Мутирует " + gen_idx + "-й ген");
            string binaryString = Convert.ToString(new_potom[gen_idx].gen, 2).PadLeft(8, '0');
            Console.WriteLine(new_potom[gen_idx].gen + " = " + binaryString);
            //ВЫбираем случайную хромосому и инвертируем
            byte number = (byte)new_potom[gen_idx].gen;

            var idx_toChange = get2nums_rnd(0, 8);
            Console.WriteLine($"Индексы меняющихся битов: {idx_toChange[0]} и {idx_toChange[1]}");
            foreach (var i in idx_toChange)
            {
                number ^= (byte)(1 << i);
            }
            Console.WriteLine(number + " = " + Convert.ToString(number, 2).PadLeft(8, '0'));
            new_potom[gen_idx].gen = number;
            get_feno_one(potom, false);
        }
        else
        {
            Console.WriteLine("Мутация не происходит\n");
        }
        return new_potom;
    }

    public void main_algorithm()
    {
        // Лог первого поколения (0)
        string genLogPath = Path.Combine(logsDir, $"generation_0.txt");
        RedirectConsoleOutput(genLogPath);

        Console.WriteLine("0 поколение");
        generate_begin();

        File.AppendAllText(fitnessFilePath,
            $"0) {string.Join(",", F_Ch_i)} = {F_Ch_i.Min()}\n", Encoding.UTF8);

        print_F_ch_i(F_Ch_i);

        RestoreConsoleOutput();

        int counter = 1;

        while (counter < N_lim)
        {
            string genLogPathCycle = Path.Combine(logsDir, $"generation_{number_o_generation}.txt");
            RedirectConsoleOutput(genLogPathCycle);

            Console.WriteLine($"Генерируется поколение {number_o_generation}");
            var new_generation = generate_new_gen(number_o_generation % Generation_coef.Count);
            Console.WriteLine($"Сгенерировано поколение {number_o_generation}");
            var new_F_ch_i = get_vector_survive(new_generation);
            print_F_ch_i(new_F_ch_i);

            File.AppendAllText(fitnessFilePath,
                $"{number_o_generation}) {string.Join(",", new_F_ch_i)} = {new_F_ch_i.Min()}\n", Encoding.UTF8);

            RestoreConsoleOutput();

            if (F_Ch_i.Min() == new_F_ch_i.Min())
            {
                counter++;
            }
            else
            {
                counter = 1;
            }

            Ch_i = new_generation;
            F_Ch_i = new_F_ch_i;
            number_o_generation++;
        }

        // Финальный вывод
        Console.WriteLine("\nПравильный ответ:");
        var idx_min = F_Ch_i.IndexOf(F_Ch_i.Min());
        print_counting_geno_one(Ch_i[idx_min]);
        get_feno_one(Ch_i[idx_min]);
    }

}

class lab6
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.Write("Введите число процессоров N = ");
        int N = Convert.ToInt16(Console.ReadLine());

        Console.Write("\nВведите число задач M = ");
        int M = Convert.ToInt16(Console.ReadLine());

        Console.Write("\nВведите нижнюю границу T1 = ");
        int T1 = Convert.ToInt16(Console.ReadLine());

        Console.Write("\nВведите нижнюю границу T2 = ");
        int T2 = Convert.ToInt16(Console.ReadLine());

        Console.Write("\nВведите число особей N_chr = ");
        int N_chr = Convert.ToInt16(Console.ReadLine());

        Console.Write("\nВведите число повторений N_lim = ");
        int N_lim = Convert.ToInt16(Console.ReadLine());

        Console.Write("\nВведите вероятность кроссинговера P_cross = ");
        double p_cross = Convert.ToDouble(Console.ReadLine());

        Console.Write("\nВведите вероятность мутации P_mutation = ");
        double p_mutation = Convert.ToDouble(Console.ReadLine());

        Console.Write("\nВведите коэффициенты поколенческой стратегии = ");
        var tmp = new List<int>();
        foreach (var i in Console.ReadLine()!.Split(','))
        {
                tmp.Add(Convert.ToInt32(i));
        }

        var test = new Matrix(N, M, T1, T2);
        test.print_matrix();
        var test1 = new Gen_algorith(N_chr, N_lim, p_cross, p_mutation, test, tmp);
        test1.main_algorithm();

    }
}