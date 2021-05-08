using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Glee.Drawing;
using System.Drawing.Imaging;

namespace WinFormsApp3
{
    [Serializable]
    public partial class Form1 : Form
    {
        private int n_account;
        private bool[,] graph;
        static List<string> map;

        // The graph that MSAGL accepts
        // Graph viewer engine
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void btnBrowse_Click_1(object sender, EventArgs e)
        {
          
           // OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            string readfile = File.ReadAllText(filename);
            textBox14.Text = filename;
            
            //richTextBox1.Text = readfile;

            // memproses teks
            Regex reg_exp = new Regex("[^A-Z]");
            readfile = reg_exp.Replace(readfile, " ");

            // split tiap kata pada teks ke array of string.
            string[] words = readfile.Split(
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
                graph[map.IndexOf(words[i]), map.IndexOf(words[i + 1])] = true;
                graph[map.IndexOf(words[i + 1]), map.IndexOf(words[i])] = true;
            }


            string[] mapp1 = map.ToArray();
            string[] mapp2 = map.ToArray();

            comboBox1.DataSource = mapp1;
            comboBox2.DataSource = mapp2;


            //MSAGL
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            //create a viewer object 
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graphh = new Microsoft.Msagl.Drawing.Graph("graphh");
            //create the graph content 
            for (int i = 0; i < words.Length; i += 2)
            {
                var Edge = graphh.AddEdge(words[i], words[i + 1]);
                Edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;
                Edge.Attr.ArrowheadAtSource = Microsoft.Msagl.Drawing.ArrowStyle.None;
            }
         
            viewer.Graph = graphh;
            //associate the viewer with the form 
            panel3.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            panel3.Controls.Add(viewer);
            panel3.ResumeLayout();
           
        }

        private void btnDFS_CheckedChanged(object sender, EventArgs e){}
        private void richTextBox2_TextChanged(object sender, EventArgs e){}
        private void richTextBox1_TextChanged(object sender, EventArgs e){}
        private void textBox7_TextChanged(object sender, EventArgs e) {}
        private void label1_Click(object sender, EventArgs e) {}
        private void button1_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private int getIdOf(string account)
        {
            return map.IndexOf(account);
        }

        public List<int> getAllAdjNodeOf(int idx_account)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < n_account; i++)
            {
                if (graph[idx_account, i])
                {
                    list.Add(i);
                }
            }
            return list;
        }


        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //memastikan richTextBox kosong
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            richTextBox4.Text = "";
            richTextBox5.Text = "";
            
            int selectedIndex = comboBox1.SelectedIndex;
            Object selectedItem1 = comboBox1.SelectedItem;
            textBox15.Text = selectedItem1.ToString();

            //Graph G1 = new Graph(readfile);
            //G1.getFriendRecommendationBFS(selectedItem1.ToString());
            //richTextBox2.Text = (G1.getFriendRecommendationBFS(selectedItem1.ToString()));
            /* INISIALISASI */


            if (radioButton1.Checked)
            {
                int idx_account = getIdOf(selectedItem1.ToString());

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
                            if ((degree == 0 && !graph[idx_account, node_id]) ||
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
                //displayOutputFriendRecommendation(idx_account, queue, friendRecFreq);
                if (queue.Count == 0)
                {
                    richTextBox5.Text += "Tidak ada rekomendasi teman untuk " + idx_account;
                }
                else
                {
                    richTextBox1.Text += "Daftar rekomendasi teman untuk " + map[idx_account];

                    int max = friendRecFreq.Max(), idx_max = friendRecFreq.IndexOf(max);
                    while (max != 0)
                    {
                        richTextBox2.Text += "Nama akun : " + map[idx_max] + "\n";
                        richTextBox3.Text += max + " mutual friends :" + "\n";
                        foreach (List<int> l in queue)
                        {
                            if (l[l.Count - 1] == idx_max)
                            {
                                richTextBox4.Text += (map[l[1]]) + "\n"; //Tampilkan mutual friend
                            }
                        }
                        friendRecFreq[idx_max] = 0;
                        max = friendRecFreq.Max();
                        idx_max = friendRecFreq.IndexOf(max);
                        //Console.WriteLine();
                    }
                }
            }
            else if (btnDFS.Checked)
            {
                /* INISIALISASI */
                int idx_account = getIdOf(selectedItem1.ToString());

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
                //displayOutputFriendRecommendation(idx_account, stack, friendRecFreq);
                if (stack.Count == 0)
                {
                    richTextBox5.Text += "Tidak ada rekomendasi teman untuk " + idx_account;
                }
                else
                {
                    richTextBox1.Text += "Daftar rekomendasi teman untuk " + map[idx_account];

                    int max = friendRecFreq.Max(), idx_max = friendRecFreq.IndexOf(max);
                    while (max != 0)
                    {
                        richTextBox2.Text += "Nama akun : " + map[idx_max] + "\n";
                        richTextBox3.Text += max + " mutual friends :" + "\n";
                        foreach (List<int> l in stack)
                        {
                            if (l[l.Count - 1] == idx_max)
                            {
                                richTextBox4.Text += (map[l[1]]) + "\n"; //Tampilkan mutual friend
                            }
                        }
                        friendRecFreq[idx_max] = 0;
                        max = friendRecFreq.Max();
                        idx_max = friendRecFreq.IndexOf(max);
                        //Console.WriteLine();
                    }
                }
            }
        }

        private void recursiveDFS(int idx_account, int idxCurrentExpandedNode, List<bool> expanded,
            List<List<int>> stack, List<int> friendRecFreq, int degree, List<int> temp)
        {
            if (degree < 1 && stack.Count != 0)
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //memastikan richTextBox kosong
            richTextBox6.Text = "";
            textBox19.Text = "";
            richTextBox7.Text = "";
            richTextBox9.Text = "";
            richTextBox8.Text = "";

            int selectedIndex1 = comboBox2.SelectedIndex;
            Object selectedItem = comboBox2.SelectedItem;
            textBox17.Text = selectedItem.ToString();

            if (btnDFS.Checked)
            {
                /* INISIALISASI */
                int asal = map.IndexOf(textBox15.Text);
                int tujuan = map.IndexOf(selectedItem.ToString());

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

                //displayOutputExploreFriend(asal, tujuan, temp, degree);

                richTextBox6.Text += "Nama akun : " + map[asal] + " dan " + map[tujuan];
                if (!found || degree == -1)
                {
                    textBox19.Text = "Tidak ada jalur koneksi yang tersedia" + "\n" + "Anda harus memulai koneksi baru itu sendiri";
                }
                else
                {
                    richTextBox7.Text += "degree connection : " + degree;
                    richTextBox8.Text = map[temp[0]];
                    for (int i = 1; i < temp.Count; i++)
                    {
                        richTextBox9.Text += " -> " + map[temp[i]];
                    }
                    //Console.WriteLine();
                }
            }
            else if (radioButton1.Checked)
            {
                /* INISIALISASI */
                int idx_account = getIdOf(textBox15.Text);

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
                    if (temp.Last() == getIdOf(selectedItem.ToString()))
                    {
                        found = true;
                        break;
                    }
                }

                // queue.Count == 0 atau temp.Last() == getIdOf(account_tujuan)

                /* Tampilkan output */
               // displayOutputExploreFriend(getIdOf(account_asal), getIdOf(account_tujuan), temp, degree, found);

                //displayOutputExploreFriend(getIdOf(account_asal), getIdOf(account_tujuan), temp, degree, found);
                richTextBox6.Text += "Nama akun : " + textBox15.Text + " dan " + selectedItem.ToString();
                if (!found || degree == -1)
                {
                    textBox19.Text += "Tidak ada jalur koneksi yang tersedia" + "\n" + "Anda harus memulai koneksi baru itu sendiri";
                }
                else
                {
                    richTextBox7.Text += "degree connection : " + degree;
                    richTextBox8.Text = map[temp[0]];
                    for (int i = 1; i < temp.Count; i++)
                    {
                        richTextBox9.Text = " -> " + map[temp[i]];
                    }
                    //Console.WriteLine();
                }

            }
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

        private void button4_Click(object sender, EventArgs e) { }
        private void textBox14_TextChanged(object sender, EventArgs e) { }
        private void textBox15_TextChanged(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged_1(object sender, EventArgs e) { }
        private void richTextBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox16_TextChanged(object sender, EventArgs e) { }
        private void richTextBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox18_TextChanged(object sender, EventArgs e) { }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
    }
}
