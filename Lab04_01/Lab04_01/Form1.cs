using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab04_01.Model;

namespace Lab04_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB context = new StudentContextDB();
                List<Faculty> listFalculty = context.Faculty.ToList(); //lấy các khoa
                List<Student> listStudent = context.Student.ToList(); //lấy sinh viên
                FillFalcultyCombobox(listFalculty);
                BindGrid(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void FillFalcultyCombobox(List<Faculty> listFalculty)
        {
            this.cmbFaculty.DataSource = listFalculty;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";

        }
        //Hàm binding gridView từ list sinh viên
        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                var faculty = cmbFaculty.Items.Cast<Faculty>()
            .FirstOrDefault(f => f.FacultyID == item.FacultyID);
                if (faculty != null)
                {
                    dgvStudent.Rows[index].Cells[2].Value = faculty.FacultyName;
                }
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtStudentId.Text) ||
                    string.IsNullOrWhiteSpace(txtFullname.Text) ||
                    string.IsNullOrWhiteSpace(txtAverageScore.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra điểm trung bình có phải số hợp lệ
                if (!float.TryParse(txtAverageScore.Text, out float averageScore))
                {
                    MessageBox.Show("Điểm trung bình phải là số hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new StudentContextDB())
                {
                    // Kiểm tra trùng mã sinh viên
                    var find = context.Student.FirstOrDefault(s => s.StudentID == txtStudentId.Text);
                    if (find != null)
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (find == null)
                    {
                        // Tạo một đối tượng Student mới
                        find = new Student();
                        find.StudentID = txtStudentId.Text;
                        context.Student.Add(find);
                        MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    find.FullName = txtFullname.Text;
                    find.AverageScore = float.Parse(txtAverageScore.Text);
                    find.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());


                    // Thêm sinh viên vào database

                    context.SaveChanges();

                    // Hiển thị lại danh sách sinh viên
                    BindGrid(context.Student.ToList());

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {          
            try
            {
                using (var context = new StudentContextDB())
                {
                    // Lấy StudentID của dòng được chọn
                    string studentID = txtStudentId.Text;

                    // Tìm sinh viên trong cơ sở dữ liệu
                    Student studentToUpdate = context.Student.FirstOrDefault(s => s.StudentID == studentID);

                    if (studentToUpdate != null)
                    {
                        // Cập nhật thông tin sinh viên
                        studentToUpdate.FullName = txtFullname.Text;
                        studentToUpdate.AverageScore = float.Parse(txtAverageScore.Text);
                        studentToUpdate.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());

                        // Lưu thay đổi
                        context.SaveChanges();

                        // Hiển thị lại danh sách
                        BindGrid(context.Student.ToList());
                        MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new StudentContextDB())
                {
                    // Lấy StudentID của dòng được chọn
                    string studentID = txtStudentId.Text;

                    // Tìm sinh viên trong cơ sở dữ liệu
                    Student studentToDelete = context.Student.FirstOrDefault(s => s.StudentID == studentID);

                    if (studentToDelete != null)
                    {
                        // Xóa sinh viên
                        context.Student.Remove(studentToDelete);
                        context.SaveChanges();

                        // Hiển thị lại danh sách
                        BindGrid(context.Student.ToList());
                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtStudentId.Text = dgvStudent.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtFullname.Text = dgvStudent.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtAverageScore.Text = dgvStudent.Rows[e.RowIndex].Cells[3].Value.ToString();
                cmbFaculty.Text = dgvStudent.Rows[e.RowIndex].Cells[2].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadFacultyComboBox()
        {
            try
            {
                using (var context = new StudentContextDB())
                {
                    var faculties = context.Faculty.ToList();
                    cmbFaculty.DataSource = faculties;
                    cmbFaculty.DisplayMember = "FacultyName"; // Hiển thị tên khoa
                    cmbFaculty.ValueMember = "FacultyID";    // Giá trị ID
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khoa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
