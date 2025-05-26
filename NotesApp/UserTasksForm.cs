using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess.Models;
using DataAccess.Repositoies;

namespace NotesApp
{
    public partial class UserTasksForm : Form
    {
        private readonly User _currentUser;
        private readonly TaskRepository _taskRepository;
        private FlowLayoutPanel flowLayoutPanel;
        private ComboBox cmbFilter;
        private TextBox txtSearch;

        public UserTasksForm(User user, TaskRepository taskRepository)
        {
            _currentUser = user;
            _taskRepository = taskRepository;
            InitializeComponent();
            InitializeCustomComponents();
            LoadTasks();
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(800, 600);
            this.Text = $"Задачи пользователя: {_currentUser.Username}";

            // Панель поиска и фильтрации
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50
            };

            txtSearch = new TextBox
            {
                Location = new Point(10, 15),
                Size = new Size(200, 20),
                PlaceholderText = "Поиск задач..."
            };

            cmbFilter = new ComboBox
            {
                Location = new Point(220, 15),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilter.Items.AddRange(new object[] { "Все", "Новые", "В процессе", "Завершенные", "Отложенные" });
            cmbFilter.SelectedIndex = 0;

            searchPanel.Controls.AddRange(new Control[] { txtSearch, cmbFilter });


            flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30, 60, 30, 30),
                Margin = new Padding(0, 30, 0, 0),
                BackColor = Color.White
            };

            this.Controls.AddRange(new Control[] { searchPanel, flowLayoutPanel });

            // Обработчики событий
            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbFilter.SelectedIndexChanged += CmbFilter_SelectedIndexChanged;
        }

        private async void LoadTasks()
        {
            try
            {
                flowLayoutPanel.Controls.Clear();
                var tasks = await _taskRepository.GetUserTasksAsync(_currentUser.Id);

                foreach (var task in tasks)
                {
                    var taskCard = new TaskCardControl(task, _taskRepository,"User");
                    flowLayoutPanel.Controls.Add(taskCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке задач: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterTasks();
        }

        private void CmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTasks();
        }

        private void FilterTasks()
        {
            string searchText = txtSearch.Text.ToLower();
            string filterStatus = cmbFilter.SelectedItem.ToString();

            foreach (TaskCardControl card in flowLayoutPanel.Controls)
            {
                bool matchesSearch = card.Text.ToLower().Contains(searchText);
                bool matchesFilter = filterStatus == "Все" || card.Text.Contains(filterStatus);

                card.Visible = matchesSearch && matchesFilter;
            }
        }
    }
}
