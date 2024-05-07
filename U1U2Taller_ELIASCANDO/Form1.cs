using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace U1U2Taller_ELIASCANDO
{
    public partial class Form1 : Form
    {
        // Mantén una lista de palabras en lugar de un diccionario
        private List<string> originalList;

        public Form1()
        {
            InitializeComponent();
            LoadData("wordlist.json");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("CellContentClick");
            Debug.WriteLine(e.RowIndex);
            // Asegurarse de que no sea un encabezado
            if (e.RowIndex < 0) return;

            // Identificar el nombre de la columna en la que se hizo clic
            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

            // Obtener el índice de la palabra
            int index = e.RowIndex;

            if (columnName == "Editar")
            {
                // Obtener la palabra antigua
                string oldWord = dataGridView1.Rows[e.RowIndex].Cells["Texto"].Value.ToString();
                // Mostrar el cuadro de diálogo para editar la palabra
                string newWord = PromptEditWord(oldWord);

                // Verificar si se cambió la palabra y actualizarla en la lista
                if (newWord != null && newWord != oldWord)
                {
                    originalList[index] = newWord;
                    SaveData("wordlist.json");
                    dataGridView1.Rows[e.RowIndex].Cells["Texto"].Value = newWord;
                }
            }
            else if (columnName == "Eliminar")
            {
                // Confirmar eliminación y eliminar la palabra de la lista
                DialogResult result = MessageBox.Show("¿Estás seguro de eliminar esta palabra?", "Confirmar", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    originalList.RemoveAt(index);
                    SaveData("wordlist.json");
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        // Cuadro de diálogo para editar la palabra
        private string PromptEditWord(string oldWord)
        {
            using (var prompt = new Form())
            {
                prompt.Width = 400;
                prompt.Height = 150;
                prompt.Text = "Editar Palabra";
                Label textLabel = new Label() { Left = 20, Top = 20, Text = "Nueva palabra:" };
                TextBox inputBox = new TextBox() { Left = 150, Top = 20, Width = 200, Text = oldWord };
                Button confirmation = new Button() { Text = "Aceptar", Left = 150, Width = 100, Top = 60, DialogResult = DialogResult.OK };

                confirmation.Click += (sender, e) => prompt.Close();
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(confirmation);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var word = newWord.Text;

                // Nombre del archivo JSON para guardar los datos.
                string fileName = "wordlist.json";

                // Verificar si la palabra ya existe en la lista.
                if (originalList == null) originalList = new List<string>();
                Debug.WriteLine("Original List: " + originalList.Count);

                if (originalList.Contains(word))
                {
                    MessageBox.Show("La palabra ya existe");
                    return;
                }

                // Agregar el nuevo elemento a la lista.
                originalList.Add(word);

                // Actualizar el DataGridView.
                dataGridView1.Rows.Add(word);

                // Guardar la lista actualizada en un archivo JSON.
                SaveData(fileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveData(string fileName)
        {
            // Asegúrate de que la lista no sea nula
            if (originalList == null) return;

            // Serializar la lista a JSON
            string jsonString = JsonSerializer.Serialize(originalList);

            // Escribir la cadena JSON a un archivo
            File.WriteAllText(fileName, jsonString);
        }

        private void LoadData(string fileName)
        {
            try
            {
                dataGridView1.Rows.Clear();
                if (!File.Exists(fileName)) return;

                // Leer el archivo JSON y deserializar en una lista
                string jsonString = File.ReadAllText(fileName);
                originalList = JsonSerializer.Deserialize<List<string>>(jsonString);

                if (originalList == null) originalList = new List<string>();

                // Añadir cada palabra al DataGridView
                foreach (var word in originalList)
                {
                    dataGridView1.Rows.Add(word);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                File.Delete(fileName);
            }
        }
        private void searchWord_KeyDown(object sender, EventArgs e)
        {

        }
        private void searchWord_TextChanged(object sender, EventArgs e)
        {
            // Obtén el término de búsqueda
            var searchTerm = searchWord.Text;

            // Verifica que la lista original esté cargada
            if (originalList == null) return;

            // Inicializa una nueva lista filtrada
            var filteredList = new List<string>();

            // Si el término de búsqueda está vacío, muestra todas las palabras
            if (string.IsNullOrEmpty(searchTerm))
            {
                filteredList = new List<string>(originalList);
            }
            else
            {
                // Filtra la lista original
                foreach (var word in originalList)
                {
                    if (word.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        filteredList.Add(word);
                    }
                }

                // Si no se encontraron coincidencias, muestra un mensaje
                if (filteredList.Count == 0)
                {
                    MessageBox.Show("La palabra no existe");
                }
            }

            // Actualiza el DataGridView con la lista filtrada
            dataGridView1.Rows.Clear();
            foreach (var word in filteredList)
            {
                dataGridView1.Rows.Add(word);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Crear la columna para Editar
            var editColumn = new DataGridViewButtonColumn
            {
                Name = "Editar",
                Text = "Editar",
                UseColumnTextForButtonValue = true
            };

            // Crear la columna para Eliminar
            var deleteColumn = new DataGridViewButtonColumn
            {
                Name = "Eliminar",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true
            };

            // Añadir las columnas al DataGridView
            dataGridView1.Columns.Add("Texto", "Texto");
            dataGridView1.Columns.Add(editColumn);
            dataGridView1.Columns.Add(deleteColumn);
        }
    }
}
