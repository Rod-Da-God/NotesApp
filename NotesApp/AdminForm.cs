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
    public partial class AdminForm : Form
    {
        private readonly TaskRepository _taskRepository;
        private readonly UserRepo _userRepository;
        private FlowLayoutPanel flowLayoutPanel;
        private ComboBox cmbFilter;
        private TextBox txtSearch;
        private Button btnAddTask;
        private Button btnRefresh;
        private ComboBox cmbUsers;

        public AdminForm(TaskRepository taskRepository, UserRepo userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            InitializeComponent();
            InitializeCustomComponents();
            LoadTasks();
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(1000, 700);
            this.Text = "Панель администратора";
            this.BackColor = Color.WhiteSmoke;

            // Панель инструментов
            var toolPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(0, 10, 0, 10)
            };

            txtSearch = new TextBox
            {
                Location = new Point(20, 25),
                Size = new Size(200, 28),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Поиск задач..."
            };

            cmbUsers = new ComboBox
            {
                Location = new Point(240, 25),
                Size = new Size(170, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };

            cmbFilter = new ComboBox
            {
                Location = new Point(430, 25),
                Size = new Size(170, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbFilter.Items.AddRange(new object[] { "Все", "Новые", "В процессе", "Завершенные", "Отложенные" });
            cmbFilter.SelectedIndex = 0;

            btnAddTask = new Button
            {
                Location = new Point(620, 22),
                Size = new Size(140, 32),
                Text = "Добавить задачу",
                BackColor = Color.FromArgb(120, 220, 160),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnAddTask.FlatAppearance.BorderSize = 0;

            btnRefresh = new Button
            {
                Location = new Point(770, 22),
                Size = new Size(120, 32),
                Text = "Обновить",
                BackColor = Color.FromArgb(180, 220, 250),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.FlatAppearance.BorderSize = 0;

            toolPanel.Controls.AddRange(new Control[] {
                txtSearch, cmbUsers, cmbFilter, btnAddTask, btnRefresh
            });

            // Панель для карточек задач
            flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30, 60, 30, 30),
                Margin = new Padding(0, 30, 0, 0),
                BackColor = Color.White
            };

            this.Controls.AddRange(new Control[] { toolPanel, flowLayoutPanel });

            // Обработчики событий
            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbFilter.SelectedIndexChanged += CmbFilter_SelectedIndexChanged;
            btnAddTask.Click += BtnAddTask_Click;
            btnRefresh.Click += BtnRefresh_Click;
            cmbUsers.SelectedIndexChanged += CmbUsers_SelectedIndexChanged;

            LoadUsers();
        }

        private async void LoadUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            cmbUsers.Items.Clear();
            cmbUsers.Items.Add("Все пользователи");
            foreach (var user in users)
            {
                cmbUsers.Items.Add(user);
            }
            cmbUsers.DisplayMember = "Username";
            cmbUsers.SelectedIndex = 0;
        }

        private async void LoadTasks()
        {
            try
            {
                flowLayoutPanel.Controls.Clear();
                List<DataAccess.Models.Tasks> tasks;
                if (cmbUsers != null && cmbUsers.SelectedIndex > 0 && cmbUsers.SelectedItem is DataAccess.Models.User selectedUser)
                {
                    tasks = await _taskRepository.GetUserTasksAsync(selectedUser.Id);
                }
                else
                {
                    tasks = await _taskRepository.GetAllTasksAsync();
                }

                foreach (var task in tasks)
                {
                    var taskCard = new TaskCardControl(task, _taskRepository, "Admin");
                    taskCard.Tag = task;
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

        private async void BtnAddTask_Click(object sender, EventArgs e)
        {
            var addTaskForm = new AddTaskForm(_taskRepository, _userRepository);
            if (addTaskForm.ShowDialog() == DialogResult.OK)
            {
                 LoadTasks();
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadTasks();
        }

        private void FilterTasks()
        {
            string searchText = txtSearch.Text.ToLower();
            string filterStatus = cmbFilter.SelectedItem.ToString();

            foreach (Control control in flowLayoutPanel.Controls)
            {
                if (control is TaskCardControl card)
                {
                    var task = card.Tag as DataAccess.Models.Tasks;
                    if (task == null)
                        continue;

                    bool matchesSearch =
                        task.Title.ToLower().Contains(searchText) ||
                        (!string.IsNullOrEmpty(task.Description) && task.Description.ToLower().Contains(searchText));

                    bool matchesFilter = filterStatus == "Все" ||
                        (filterStatus == "Новые" && task.Status == DataAccess.Models.TaskStatus.New) ||
                        (filterStatus == "В процессе" && task.Status == DataAccess.Models.TaskStatus.InProgress) ||
                        (filterStatus == "Завершенные" && task.Status == DataAccess.Models.TaskStatus.Completed) ||
                        (filterStatus == "Отложенные" && task.Status == DataAccess.Models.TaskStatus.Postponed);

                    card.Visible = matchesSearch && matchesFilter;
                }
            }
        }

        private void CmbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTasks();
        }
    }
}