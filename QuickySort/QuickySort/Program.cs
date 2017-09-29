using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickySort
{
    class Program
    {
        
        static void Main(string[] args)
        {
           

            var list = new List<int>();
            var rnd = new Random();

            var length = 3000000;
            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine($"    Resultados para {length} itens: \n");
            
            for (var i = 0; i < length; i++)
            {
                list.Add(rnd.Next(1, length));
            }
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            Quicksort(list, 0, list.Count-1);
            watch.Stop();
            
            var elapsedMs = watch.ElapsedMilliseconds;  
            
            Console.WriteLine($"    QuickSort: {elapsedMs} ms");
            
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            
            Quicksort(list, 0, list.Count-1);
            watch2.Stop();
            
            var elapsedMs2 = watch2.ElapsedMilliseconds;  
            Console.WriteLine($"    QuickSort Parallel: {elapsedMs2} ms");
            
            Console.WriteLine("\n\n\n\n\n");
            Console.ReadLine();
            
        }
        
        private static void Swap<T>(IList<T> arr, int i, int j)
        {
            var tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }
        
        private static int Partition<T>(IList<T> arr, int low, int high)
            where T : IComparable<T>
        {
//            Definindo a posição de pivô, aqui o elemento do meio é usado, 
//            mas a escolha de um pivô é uma questão bastante complicada. 
//            Escolher o elemento esquerdo nos leva ao desempenho do pior caso, 
//            o que é um caso bastante comum, é por isso que não é usado aqui.
            var pivotPos = (high + low) / 2;
            var pivot = arr[pivotPos];
            
            
//            Colocando o pivô à esquerda da partição (índice mais baixo) para simplificar o loop
            Swap(arr, low, pivotPos);
 
//            O pivô permanece no índice mais baixo até o final do loop.
//            A variável esquerda está aqui para acompanhar o número de valores que 
//            foram comparados como "menor que" o pivô.
            var left = low;
            for (var i = low + 1; i <= high; i++)
            {
                // Se o valor for maior que o valor de pivô, continuamos para o próximo índice.
                if (arr[i].CompareTo(pivot) >= 0) continue;
 
                // Se o valor for menor do que o pivô, nós incrementamos nosso contador esquerdo (mais um elemento abaixo do pivô)
                left++;
                // ae sim, trocamos nosso elemento no nosso índice esquerdo.
                Swap(arr, i, left);
            }
 
            // O pivô ainda está no índice mais baixo, precisamos colocá-lo de volta após todos os valores encontrados serem "inferiores" ao pivô.
            Swap(arr, low, left);
 
            // Retornamos o novo índice do nosso pivô
            return left;
        }
        
        private static void Quicksort<T>(IList<T> arr, int left, int right) where T : IComparable<T>
        {
            // Se a lista contiver um ou menos elementos: não é necessário classificar!
            if (right <= left) return;
 
            // Particionando a lista
            var pivot = Partition(arr, left, right);
 
            // Ordenando a esquerda do pivô
            Quicksort(arr, left, pivot - 1);
            // Classificando a direita do pivô
            Quicksort(arr, pivot + 1, right);
        }
        
        private static void QuicksortParallel<T>(IList<T> arr, int left, int right)
            where T : IComparable<T>
        {
//            Definindo um comprimento mínimo para usar paralelismo, sobre o qual usando paralelismo
//            melhorou o desempenho do que a versão sequencial.
            const int threshold = 2048;
 
            // Se a lista para ordenar contém um ou menos elementos, a lista já está classificada.
            if (right <= left) return;
 
            // Se o tamanho da lista estiver abaixo do limite, a versão seqüencial será usada.
            if (right - left < threshold)
                Quicksort(arr, left, right);
 
            else
            {
                // Particionando nossa lista e obtendo um pivô.
                var pivot = Partition(arr, left, right);
 
                // Classificando o lado esquerdo e direito do pivô em paralelo
                Parallel.Invoke(
                    () => QuicksortParallel(arr, left, pivot - 1),
                    () => QuicksortParallel(arr, pivot + 1, right));
            }
        }
    }
}