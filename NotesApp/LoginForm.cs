using AuthLibrary;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositoies;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace NotesApp
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _db;

        public LoginForm()
        {
            _db = new AppDbContext();
            var userRepo = new UserRepo(_db);
            _authService = new AuthService(userRepo);

            InitializeComponent();
            
            ApplyCustomStyle();

            button1.Click += async (s, e) => await BtnLogin_Click();
            button2.Click += async (s, e) => await BtnRegister_Click();
            button3.Click += async (s, e) => await TestDatabase();
        }
        
        private void ApplyCustomStyle()
        {
            this.Text = "Авторизация в системе";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 250);
            this.Size = new Size(450, 400);
            
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 250),
                Padding = new Padding(20)
            };
            this.Controls.Add(mainPanel);
            
            Label titleLabel = new Label
            {
                Text = "Вход в NotesApp",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 110),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 50),
                Location = new Point(25, 20)
            };
            mainPanel.Controls.Add(titleLabel);
            
            this.Controls.Remove(button1);
            this.Controls.Remove(button2);
            this.Controls.Remove(button3);
            this.Controls.Remove(label1);
            this.Controls.Remove(label2);
            this.Controls.Remove(textBox1);
            this.Controls.Remove(textBox2);
            
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11);
            label1.ForeColor = Color.FromArgb(60, 60, 110);
            label1.Text = "Имя пользователя:";
            label1.Location = new Point(60, 100);
            
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11);
            label2.ForeColor = Color.FromArgb(60, 60, 110);
            label2.Text = "Пароль:";
            label2.Location = new Point(60, 160);
            
            // Настраиваем текстовые поля
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new Font("Segoe UI", 11);
            textBox1.Size = new Size(270, 30);
            textBox1.Location = new Point(60, 125);
            
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.Font = new Font("Segoe UI", 11);
            textBox2.Size = new Size(270, 30);
            textBox2.Location = new Point(60, 185);
            
            // Настраиваем кнопки
            button1.FlatStyle = FlatStyle.Flat;
            button1.BackColor = Color.FromArgb(79, 117, 240);
            button1.ForeColor = Color.White;
            button1.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            button1.Text = "Войти";
            button1.Size = new Size(120, 40);
            button1.Location = new Point(60, 240);
            button1.FlatAppearance.BorderSize = 0;
            button1.Cursor = Cursors.Hand;
            
            button2.FlatStyle = FlatStyle.Flat;
            button2.BackColor = Color.FromArgb(230, 230, 250);
            button2.ForeColor = Color.FromArgb(60, 60, 110);
            button2.Font = new Font("Segoe UI", 11);
            button2.Text = "Регистрация";
            button2.Size = new Size(120, 40);
            button2.Location = new Point(210, 240);
            button2.FlatAppearance.BorderSize = 0;
            button2.Cursor = Cursors.Hand;
            
            button3.FlatStyle = FlatStyle.Flat;
            button3.BackColor = Color.White;
            button3.ForeColor = Color.Gray;
            button3.Font = new Font("Segoe UI", 9);
            button3.Text = "Тест базы данных";
            button3.Size = new Size(140, 30);
            button3.Location = new Point(135, 300);
            button3.FlatAppearance.BorderColor = Color.LightGray;
            button3.Cursor = Cursors.Hand;
            
            // Добавляем все контролы в панель
            mainPanel.Controls.Add(label1);
            mainPanel.Controls.Add(textBox1);
            mainPanel.Controls.Add(label2);
            mainPanel.Controls.Add(textBox2);
            mainPanel.Controls.Add(button1);
            mainPanel.Controls.Add(button2);
            mainPanel.Controls.Add(button3);
            
            // Добавляем тени и закругленные углы для кнопок
            MakeRoundedControl(button1, 10);
            MakeRoundedControl(button2, 10);
            MakeRoundedControl(textBox1, 5);
            MakeRoundedControl(textBox2, 5);
        }

        // Вспомогательный метод для создания закругленных углов
        private void MakeRoundedControl(Control control, int radius)
        {
            control.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius));
        }


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, 
            int nTopRect, 
            int nRightRect, 
            int nBottomRect, 
            int nWidthEllipse, 
            int nHeightEllipse
        );

        private async Task TestDatabase()
        {
            try
            {
                bool canConnect = _db.Database.CanConnect();
                
                if (!canConnect)
                {
                    MessageBox.Show("Не удалось подключиться к базе данных!", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                User testUser = new User
                {
                    Username = "test_" + DateTime.Now.Ticks.ToString().Substring(10),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
                    Role = UserRole.User
                };
                
                _db.Users.Add(testUser);
                
                try
                {
                    await _db.SaveChangesAsync();
                    MessageBox.Show($"Тестовый пользователь успешно создан!\nЛогин: {testUser.Username}\nПароль: test", 
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    var fullError = ex.ToString(); 
                    MessageBox.Show($"Ошибка при сохранении данных:\n{fullError}", 
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка:\n{ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task BtnLogin_Click()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль", 
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            User user = await _authService.LoginAsync(textBox1.Text, textBox2.Text);
            if (user != null)
            {
                OpenMainForm(user);
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", 
                    "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task BtnRegister_Click()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Пожалуйста, введите логин и пароль", 
                        "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == textBox1.Text);
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким именем уже существует", 
                        "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                var newUser = new User
                {
                    Username = textBox1.Text,
                    Role = UserRole.User
                };

                var success = await _authService.RegisterAsync(newUser, textBox2.Text);
                
                if (success)
                {
                    MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.",
                        "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Очищаем поля после успешной регистрации
                    textBox1.Clear();
                    textBox2.Clear();
                }
                else
                {
                    MessageBox.Show("Не удалось зарегистрировать пользователя", 
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                var fullError = ex.ToString(); 
                MessageBox.Show($"Ошибка регистрации:\n{ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenMainForm(User user)
        {
            var taskRepository = new TaskRepository(_db);
            var userRepository = new UserRepo(_db);

            if (user.Role == UserRole.Admin)
            {
                var adminForm = new AdminForm(taskRepository, userRepository);
                adminForm.Show();
            }
            else
            {
                var userForm = new UserTasksForm(user, taskRepository);
                userForm.Show();
            }

            this.Hide();
        }
    }
}