using System;
using System.Drawing;
using System.Windows.Forms;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositoies;

namespace NotesApp
{
    public partial class AddTaskForm : Form
    {
        TaskRepository _taskRepository;
        UserRepo _userRepository;
        ComboBox cmbUsers;
        TextBox txtTitle;
        TextBox txtDescription;
        DateTimePicker dtpDueDate;

        public AddTaskForm(TaskRepository taskRepository, UserRepo userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(400, 300);
            this.Text = "Добавить новую задачу";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Заголовок
            var lblTitle = new Label
            {
                Location = new Point(10, 15),
                Size = new Size(100, 20),
                Text = "Заголовок:"
            };

            txtTitle = new TextBox
            {
                Location = new Point(120, 12),
                Size = new Size(250, 20)
            };

            // Описание
            var lblDescription = new Label
            {
                Location = new Point(10, 45),
                Size = new Size(100, 20),
                Text = "Описание:"
            };

            txtDescription = new TextBox
            {
                Location = new Point(120, 42),
                Size = new Size(250, 60),
                Multiline = true
            };

            // Срок выполнения
            var lblDueDate = new Label
            {
                Location = new Point(10, 115),
                Size = new Size(100, 20),
                Text = "Срок:"
            };

            dtpDueDate = new DateTimePicker
            {
                Location = new Point(120, 112),
                Size = new Size(250, 20),
                Format = DateTimePickerFormat.Short
            };

            // Пользователь
            var lblUser = new Label
            {
                Location = new Point(10, 145),
                Size = new Size(100, 20),
                Text = "Пользователь:"
            };

            cmbUsers = new ComboBox
            {
                Location = new Point(120, 142),
                Size = new Size(250, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Кнопки
            var btnSave = new Button
            {
                Location = new Point(120, 180),
                Size = new Size(100, 30),
                Text = "Сохранить",
                DialogResult = DialogResult.OK
            };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button
            {
                Location = new Point(230, 180),
                Size = new Size(100, 30),
                Text = "Отмена",
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] {
                lblTitle, txtTitle,
                lblDescription, txtDescription,
                lblDueDate, dtpDueDate,
                lblUser, cmbUsers,
                btnSave, btnCancel
            });

            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                cmbUsers.DataSource = users;
                cmbUsers.DisplayMember = "Username";
                cmbUsers.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите заголовок задачи", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbUsers.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new AppDbContext()) {
                    var task = new Tasks
                    {
                        Title = txtTitle.Text,
                        Description = txtDescription.Text,
                        DueDate = dtpDueDate.Value.ToUniversalTime(),
                        Status = DataAccess.Models.TaskStatus.New
                    };

                    var userId = ((User)cmbUsers.SelectedItem).Id;
                    await _taskRepository.AssignTaskAsync(task, userId);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                string details = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Ошибка при сохранении задачи: {details}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 