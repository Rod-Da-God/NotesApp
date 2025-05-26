using System;
using System.Drawing;
using System.Windows.Forms;
using DataAccess.Models;
using DataAccess.Repositoies;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NotesApp
{
    public partial class TaskCardControl : UserControl
    {
        private readonly Tasks _task;
        private readonly TaskRepository _taskRepository;
        private Label lblTitle;
        private Label lblDescription;
        private Label lblDueDate;
        private Label lblStatus;
        private Button btnComplete;
        private Button btnPostpone;
        private Button btnDelete;
        private string role;

        public TaskCardControl(Tasks task, TaskRepository taskRepository, string r)
        {
            _task = task;
            _taskRepository = taskRepository;
            role = r;
            InitializeComponent();
            InitializeControls();
            UpdateTaskInfo();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(320, 160);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.None;
            this.Margin = new Padding(10, 20, 10, 20);
            this.Font = new Font("Segoe UI", 10);
            this.Paint += TaskCardControl_Paint;
        }

        private void InitializeControls()
        {
            lblTitle = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(280, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lblDescription = new Label
            {
                Location = new Point(10, 35),
                Size = new Size(280, 40),
                Font = new Font("Segoe UI", 9)
            };

            lblDueDate = new Label
            {
                Location = new Point(10, 80),
                Size = new Size(140, 20),
                Font = new Font("Segoe UI", 8)
            };

            lblStatus = new Label
            {
                Location = new Point(150, 80),
                Size = new Size(140, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = GetStatusColor(_task.Status)
            };

            btnComplete = new Button
            {
                Location = new Point(10, 110),
                Size = new Size(90, 30),
                Text = "Выполнить",
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            btnPostpone = new Button
            {
                Location = new Point(110, 110),
                Size = new Size(90, 30),
                Text = "Отложить",
                BackColor = Color.LightYellow,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            btnDelete = new Button
            {
                Location = new Point(210, 110),
                Size = new Size(90, 30),
                Text = "Удалить",
                BackColor = Color.LightCoral,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Visible = _taskRepository != null // Показывать только для админа
            };

            this.Controls.AddRange(new Control[] { 
                lblTitle, lblDescription, lblDueDate, lblStatus, 
                btnComplete, btnPostpone, btnDelete
            });

            btnComplete.Click += BtnComplete_Click;
            btnPostpone.Click += BtnPostpone_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private void UpdateTaskInfo()
        {
            lblTitle.Text = _task.Title;
            lblDescription.Text = _task.Description;
            lblDueDate.Text = $"Срок: {_task.DueDate:dd.MM.yyyy}";
            lblStatus.Text = $"Статус: {_task.Status}";
            lblStatus.ForeColor = GetStatusColor(_task.Status);
        }

        private Color GetStatusColor(DataAccess.Models.TaskStatus status)
        {
            return status switch
            {
                DataAccess.Models.TaskStatus.New => Color.Blue,
                DataAccess.Models.TaskStatus.InProgress => Color.Orange,
                DataAccess.Models.TaskStatus.Completed => Color.Green,
                DataAccess.Models.TaskStatus.Failed => Color.Red,
                DataAccess.Models.TaskStatus.Postponed => Color.Gray,
                _ => Color.Black
            };
        }

        private async void BtnComplete_Click(object sender, EventArgs e)
        {
            try
            {
                await _taskRepository.UpdateTaskStatusAsync(_task.Id, DataAccess.Models.TaskStatus.Completed);
                _task.Status = DataAccess.Models.TaskStatus.Completed;
                UpdateTaskInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnPostpone_Click(object sender, EventArgs e)
        {
            try
            {
                await _taskRepository.UpdateTaskStatusAsync(_task.Id, DataAccess.Models.TaskStatus.Postponed);
                _task.Status = DataAccess.Models.TaskStatus.Postponed;
                UpdateTaskInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить задачу?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes && role == "Admin")
            {
                try
                {
                    await _taskRepository.DeleteTaskAsync(_task.Id);
                    this.Parent?.Controls.Remove(this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Удалить задачу может только администратор");
            }
        }

        private void TaskCardControl_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var rect = this.ClientRectangle;
            rect.Inflate(-2, -2);
            int radius = 18;
            using (var path = RoundedRect(rect, radius))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                // Тень
                var shadowRect = rect;
                shadowRect.Offset(3, 3);
                g.FillPath(shadowBrush, RoundedRect(shadowRect, radius));
                // Фон
                using (var bgBrush = new SolidBrush(Color.White))
                    g.FillPath(bgBrush, path);
                // Рамка
                using (var pen = new Pen(Color.LightGray, 1))
                    g.DrawPath(pen, path);
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
} 