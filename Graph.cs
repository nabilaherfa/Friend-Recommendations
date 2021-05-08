using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
//using System.Data;


namespace WinFormsApp3
{

    public class Graph
    {
        private int n_account;
        private bool[,] graph;
        private List<string> map;

        public Graph(string teks)
        {
            // memproses teks
            Regex reg_exp = new Regex("[^A-Z]");
            teks = reg_exp.Replace(teks, " ");
            
            // split tiap kata pada teks ke array of string.
            string[] words = teks.Split(
               new char[] { ' ' },
               StringSplitOptions.RemoveEmptyEntries);


            // mendapatkan unique char
            var word_query =
               (from string word in words
                orderby word
                select word).Distinct();

            // masukin ke variable
            map = word_query.ToList();
            n_account = map.Count;

            graph = new bool[n_account, n_account];

            for (int i = 0; i < words.Length; i += 2)
            {
                graph[map.IndexOf(words[i]), map.IndexOf(words[i+1])] = true;
                graph[map.IndexOf(words[i+1]), map.IndexOf(words[i])] = true;
            }
        }

        private int getIdOf(string account)
        {
            return map.IndexOf(account);
        }

        public List<int> getAllAdjNodeOf(int idx_account)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < n_account; i++)
            {
                if (graph[idx_account,i])
                {
                    list.Add(i);
                }
            }
            return list;
        }

        public void getFriendRecommendationBFS(string account)
        {
            /* INISIALISASI */
            int idx_account = getIdOf(account);

            /* Mencatat index yang sedang diekspan*/
            int idxCurrentExpandedNode = idx_account;

            /* queue merupakan list berelemen list of int dimana tiap int secara
             * berurutan menyatakan path dari idx_account ke indeks dari simpul
             * hidup tertentu (pada elemen terakhir)
             */
            List<List<int>> queue = new List<List<int>>();

            List<int> temp = new List<int>(n_account);
            int degree = -1; /*Mencatat derajat simpul yang sedang diekspan
                              -1 untuk simpul asal (idx_account) dan bertambah 1
                              tiap kenaikan derajat*/

            /* Inisialisasi list untuk mencatat frekuensi kemunculan suatu akun
             * dengan indeks akun yang bersesuaian dengan indeksnya pada list
             * sebagai kandidat teman yang direkomendasikan */
            List<int> friendRecFreq = new List<int>(n_account);


            /* Inisialisasi array expanded untuk mencatat status tiap simpul sudah
            diekspan atau belum */
            List<bool> expanded = new List<bool>();

            for (int i = 0; i < n_account; i++)
            {
                if (i == idxCurrentExpandedNode)
                {
                    expanded.Add(true);
                    temp.Add(idxCurrentExpandedNode);
                    queue.Add(new List<int>(temp));
                }
                else
                {
                    expanded.Add(false);
                }
                friendRecFreq.Add(0);
            }

            /*Proses Pencarian Rekomendasi Teman secara BFS*/
            while ((queue.Count != 0) && (degree < 1))
            {
                /* pencarian akun yang berteman dengan node yang sedang diekspan
                 dan memenuhi syarat ekspansi */
                queue.RemoveAt(0);
                foreach (int node_id in getAllAdjNodeOf(idxCurrentExpandedNode))
                {
                    if (!expanded[node_id])
                    {
                        if ((degree == 0 && !graph[idx_account, node_id])||
                            (degree != 0))
                        {
                            /* Push ke antrian */
                            temp.Add(node_id);
                            queue.Add(new List<int>(temp));
                            temp.RemoveAt(temp.Count - 1);
                            if (degree == 0)
                            {
                                friendRecFreq[node_id]++;
                            }

                        }
                    }
                }

                /* Tandai idxCurrentExpandedNode sudah diekspan */
                expanded[idxCurrentExpandedNode] = true;

                /* Lanjut ekspan simpul selanjutnya yang dipilih mengikuti metode BFS */
                if (queue.Count != 0)
                {
                    temp.Clear();
                    temp = queue[0];
                    degree = temp.Count - 2;
                    idxCurrentExpandedNode = temp[temp.Count - 1];
                }
            }

            // queue.Count == 0 atau degree == 1

            /* Tampilkan output */
            displayOutputFriendRecommendation(idx_account, queue, friendRecFreq);
        }

        public void getFriendRecommendationDFS(string account)
        {
            /* INISIALISASI */
            int idx_account = getIdOf(account);

            /* Mencatat index yang sedang diekspan*/
            int idxCurrentExpandedNode = idx_account;

            /* queue merupakan list berelemen list of int dimana tiap int secara
             * berurutan menyatakan path dari idx_account ke indeks dari simpul
             * hidup tertentu (pada elemen terakhir)
             */
            List<List<int>> stack = new List<List<int>>();

            List<int> temp = new List<int>();
            int degree = -1; /*Mencatat derajat simpul yang sedang diekspan
                              -1 untuk simpul asal (idx_account) dan bertambah 1
                              tiap kenaikan derajat*/

            /* Inisialisasi array untuk mencatat status tiap simpul sudah
            diekspan atau belum */
            List<bool> expanded = new List<bool>();

            /* Inisialisasi list untuk mencatat frekuensi kemunculan suatu akun
             * sebagai kandidat teman yang direkomendasikan,
             * dengan indeks akun  bersesuaian dengan indeksnya pada list*/
            List<int> friendRecFreq = new List<int>(n_account);

            for (int i = 0; i < n_account; i++)
            {
                expanded.Add(false);
                friendRecFreq.Add(0);
            }

            /* proses awal*/
            temp.Add(idxCurrentExpandedNode);
            stack.Add(new List<int>(temp));

            recursiveDFS(idx_account, idxCurrentExpandedNode, expanded, stack, friendRecFreq,
                degree, temp);

            /* Menampilkan output */
            displayOutputFriendRecommendation(idx_account,stack,friendRecFreq);
        }

        private void recursiveDFS(int idx_account, int idxCurrentExpandedNode, List<bool> expanded,
            List<List<int>> stack, List<int> friendRecFreq, int degree, List<int> temp)
        {
            if (degree<1 && stack.Count != 0)
            {
                stack.RemoveAt(stack.Count - 1);
                /* Tandai idxCurrentExpandedNode sudah diekspan */
                expanded[idxCurrentExpandedNode] = true;

                foreach (int node_id in getAllAdjNodeOf(idxCurrentExpandedNode))
                {

                    if (!expanded[node_id])
                    {
                        if ((degree == 0 && !graph[idx_account, node_id]) ||
                            (degree != 0))
                        {
                            /* Push ke stack */
                            temp.Add(node_id);
                            stack.Add(new List<int>(temp));
                            if (degree == 0)
                            {
                                friendRecFreq[node_id]++;
                            }
                            /* Lanjut ekspan simpul selanjutnya yang dipilih mengikuti metode DFS */
                            degree = temp.Count - 2;
                            idxCurrentExpandedNode = temp[temp.Count - 1];

                            recursiveDFS(idx_account, idxCurrentExpandedNode, expanded, stack,
                                friendRecFreq, degree, temp);
                            temp.RemoveAt(temp.Count - 1);
                            degree = temp.Count - 2;
                        }
                    }
                }
            }
        }

        private void displayOutputFriendRecommendation(int idx_account,
            List<List<int>> listAccount, List<int> friendRecFreq)
        {
            if (listAccount.Count == 0)
            {
                Console.WriteLine("Tidak ada rekomendasi teman untuk "+idx_account);
            }
            else
            {
                Console.WriteLine("Daftar rekomendasi teman untuk " + map[idx_account]);

                int max = friendRecFreq.Max(), idx_max = friendRecFreq.IndexOf(max);
                while (max != 0)
                {
                    Console.WriteLine("Nama akun : "+map[idx_max]);
                    Console.WriteLine(max+" mutual friends :");
                    foreach (List<int> l in listAccount)
                    {
                        if (l[l.Count - 1] == idx_max)
                        {
                            Console.WriteLine(map[l[1]]); //Tampilkan mutual friend
                        }
                    }
                    friendRecFreq[idx_max] = 0;
                    max = friendRecFreq.Max();
                    idx_max = friendRecFreq.IndexOf(max);
                    Console.WriteLine();
                }
            }
        }

        public void exploreFriendsBFS(string account_asal, string account_tujuan)
        {


            /* INISIALISASI */
            int idx_account = getIdOf(account_asal);

            /* Mencatat index yang sedang diekspan*/
            int idxCurrentExpandedNode = idx_account;

            /* queue merupakan list berelemen list of int dimana tiap int secara
             * berurutan menyatakan path dari idx_account ke indeks dari simpul
             * hidup tertentu (pada elemen terakhir)
             */
            List<List<int>> queue = new List<List<int>>();

            List<int> temp = new List<int>(n_account);
            int degree = -1; /*Mencatat derajat simpul yang sedang diekspan
                              -1 untuk simpul asal (idx_account) dan bertambah 1
                              tiap kenaikan derajat*/

            /* Inisialisasi array expanded untuk mencatat status tiap simpul sudah
            diekspan atau belum */
            List<bool> expanded = new List<bool>();

            for (int i = 0; i < n_account; i++)
            {
                if (i == idxCurrentExpandedNode)
                {
                    expanded.Add(true);
                    temp.Add(idxCurrentExpandedNode);
                    queue.Add(new List<int>(temp));
                }
                else
                {
                    expanded.Add(false);
                }
            }

            /*Proses Pencarian Rekomendasi Teman secara BFS*/
            bool found = false;
            while ((queue.Count != 0))
            {
                /* pencarian akun yang berteman dengan node yang sedang diekspan
                 dan memenuhi syarat ekspansi */
                queue.RemoveAt(0);
                foreach (int node_id in getAllAdjNodeOf(idxCurrentExpandedNode))
                {
                    if (!expanded[node_id]) // Jika belum diekspan
                    {
                        /* Push ke antrian */
                        temp.Add(node_id);
                        queue.Add(new List<int>(temp));
                        temp.RemoveAt(temp.Count - 1);
                    }
                }

                /* Tandai idxCurrentExpandedNode sudah diekspan */
                expanded[idxCurrentExpandedNode] = true;

                /* Lanjut ekspan simpul selanjutnya yang dipilih mengikuti metode BFS */
                if (queue.Count != 0)
                {
                    temp.Clear();
                    temp = queue[0];
                    degree = temp.Count - 2;
                    idxCurrentExpandedNode = temp[temp.Count - 1];
                }

                /* Jika elemen terakhir temp adalah akun tujuan, maka while dihentikan (karena sudah ketemu) */
                if (temp.Last() == getIdOf(account_tujuan))
                {
                    found = true;
                    break;
                }
            }

            // queue.Count == 0 atau temp.Last() == getIdOf(account_tujuan)

            /* Tampilkan output */
            displayOutputExploreFriend(getIdOf(account_asal), getIdOf(account_tujuan), temp, degree, found);
        }

        public void exploreFriendsDFS(string account_asal, string account_tujuan)
        {
            /* INISIALISASI */
            int asal = map.IndexOf(account_asal);
            int tujuan = map.IndexOf(account_tujuan);

            /* Mencatat index yang sedang diekspan*/
            int idxCurrentExpandedNode = asal;

            /* stack merupakan list berelemen list of int dimana tiap int secara
             * berurutan menyatakan path dari idx_account ke indeks dari simpul
             * hidup tertentu (pada elemen terakhir)
             */
            List<List<int>> stack = new List<List<int>>();

            List<int> temp = new List<int>();
            int degree = -1; /*Mencatat derajat simpul yang sedang diekspan
                              -1 untuk simpul asal (idx_account) dan bertambah 1
                              tiap kenaikan derajat*/

            /* Inisialisasi array untuk mencatat status tiap simpul sudah
            diekspan atau belum */
            List<bool> expanded = new List<bool>();

            /* Inisialisasi list untuk mencatat frekuensi kemunculan suatu akun
             * sebagai kandidat teman yang direkomendasikan,
             * dengan indeks akun  bersesuaian dengan indeksnya pada list*/
            //List<int> friendRecFreq = new List<int>(n_account);

            for (int i = 0; i < n_account; i++)
            {
                expanded.Add(false);
            }

            /* proses awal*/
            temp.Add(idxCurrentExpandedNode);
            stack.Add(new List<int>(temp));
            bool found = false;

            recursiveDFS2(asal, tujuan, idxCurrentExpandedNode, expanded, stack,
               ref degree, temp, ref found);

            displayOutputExploreFriend(asal, tujuan, temp, degree, found);
        }

        private void recursiveDFS2(int idx_asal, int idx_tujuan, int idxCurrentExpandedNode, List<bool> expanded,
            List<List<int>> stack, ref int degree, List<int> temp, ref bool found)
        {
            if (idxCurrentExpandedNode != idx_tujuan && stack.Count != 0)
            {
                stack.RemoveAt(stack.Count - 1);
                /* Tandai idxCurrentExpandedNode sudah diekspan */
                expanded[idxCurrentExpandedNode] = true;

                List<int> adjnode = getAllAdjNodeOf(idxCurrentExpandedNode);
                int i = 0;
                while (!found && i < adjnode.Count)
                {
                    if (!expanded[adjnode[i]])
                    {
                        /* Push ke stack */
                        temp.Add(adjnode[i]);
                        stack.Add(new List<int>(temp));

                        /* Lanjut ekspan simpul selanjutnya yang dipilih mengikuti metode DFS */
                        degree = temp.Count - 2;
                        idxCurrentExpandedNode = temp[temp.Count - 1];


                        recursiveDFS2(idx_asal, idx_tujuan, idxCurrentExpandedNode, expanded, stack, ref degree, temp, ref found);
                        if (!found)
                        {
                            temp.RemoveAt(temp.Count - 1);
                            degree = temp.Count - 2;
                        }
                    }
                    i++;
                }
            }
            if (idxCurrentExpandedNode == idx_tujuan)
            {
                found = true;
            }
        }
        private void displayOutputExploreFriend(int asal, int tujuan, List<int> temp, int degree, bool found)
        {
            Console.WriteLine("Nama akun : " + map[asal] + " dan " + map[tujuan]);
            if (!found)
            {
                Console.WriteLine("Tidak ada jalur koneksi yang tersedia");
                Console.WriteLine("Anda harus memulai koneksi baru itu sendiri");
            }
            else
            {
                Console.WriteLine("degree connection : " + degree);
                Console.Write(map[temp[0]]);
                for (int i = 1; i < temp.Count; i++)
                {
                    Console.Write(" -> " + map[temp[i]]);
                }
                Console.WriteLine();
            }

        }
    }
}