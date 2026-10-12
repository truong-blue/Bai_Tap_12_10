using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai4._7
{
    public partial class FormStudent : Form
    {
        public event Action<string> OnThongBaoTrangThai;
        private BindingList<Student> _studentList = new BindingList<Student>();
        private BindingSource _bindingSource = new BindingSource();

        public FormStudent()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Thiết lập ComboBox giới tính
            cboGender.Items.Clear();
            cboGender.Items.AddRange(new object[] { "Nam", "Nữ" });

            // Nạp dữ liệu mẫu ban đầu
            KhoiTaoDuLieuMau();

            // Kết nối BindingSource với danh sách
            _bindingSource.DataSource = _studentList;
            dgvStudents.AutoGenerateColumns = true;
            dgvStudents.DataSource = _bindingSource;

            // Thiết lập DataBinding hai chiều
            ThietLapDataBinding();

            // Cấu hình hiển thị DataGridView
            CauHinhDataGridView();

            // Tự động cập nhật ảnh khi chuyển chọn dòng khác
            _bindingSource.CurrentChanged += BindingSource_CurrentChanged;

            // Nạp ảnh cho dòng đầu tiên
            BindingSource_CurrentChanged(null, EventArgs.Empty);
        }

        private void KhoiTaoDuLieuMau()
        {
            _studentList.Add(new Student("SV01", "Nguyễn Văn An", 20, "Nam", ""));
            _studentList.Add(new Student("SV02", "Trần Thị Mai", 19, "Nữ", ""));
            _studentList.Add(new Student("SV03", "Lê Hoàng Long", 21, "Nam", ""));
        }

        // ==============================================================
        // CẤU HÌNH LIÊN KẾT DỮ LIỆU HAI CHIỀU (DATA BINDINGS 2 CHIỀU)
        // ==============================================================
        private void ThietLapDataBinding()
        {
            txtStudentId.DataBindings.Clear();
            txtFullName.DataBindings.Clear();
            txtAge.DataBindings.Clear();
            cboGender.DataBindings.Clear();
            txtAvatarPath.DataBindings.Clear();

            // DataSourceUpdateMode.OnPropertyChanged: gõ là đồng bộ sang object ngay lập tức
            txtStudentId.DataBindings.Add("Text", _bindingSource, "StudentId", true, DataSourceUpdateMode.OnPropertyChanged);
            txtFullName.DataBindings.Add("Text", _bindingSource, "FullName", true, DataSourceUpdateMode.OnPropertyChanged);
            txtAge.DataBindings.Add("Text", _bindingSource, "Age", true, DataSourceUpdateMode.OnPropertyChanged);
            cboGender.DataBindings.Add("SelectedItem", _bindingSource, "Gender", true, DataSourceUpdateMode.OnPropertyChanged);
            txtAvatarPath.DataBindings.Add("Text", _bindingSource, "AvatarPath", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void CauHinhDataGridView()
        {
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.MultiSelect = false;
            dgvStudents.ReadOnly = true;

            if (dgvStudents.Columns["StudentId"] != null) dgvStudents.Columns["StudentId"].HeaderText = "Mã SV";
            if (dgvStudents.Columns["FullName"] != null) dgvStudents.Columns["FullName"].HeaderText = "Họ và Tên";
            if (dgvStudents.Columns["Age"] != null) dgvStudents.Columns["Age"].HeaderText = "Tuổi";
            if (dgvStudents.Columns["Gender"] != null) dgvStudents.Columns["Gender"].HeaderText = "Giới tính";
            if (dgvStudents.Columns["AvatarPath"] != null) dgvStudents.Columns["AvatarPath"].HeaderText = "Đường dẫn ảnh";
        }

        // ==============================================================
        // ĐỔI ẢNH TỰ ĐỘNG THEO DÒNG ĐANG CHỌN (TRÁNH LỖI KHÓA FILE)
        // ==============================================================
        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (_bindingSource.Current is Student currentStudent)
            {
                LoadAvatarKhongKhoaFile(currentStudent.AvatarPath);
            }
            else
            {
                XoaAnhHienTai();
            }
        }

        private void LoadAvatarKhongKhoaFile(string imagePath)
        {
            XoaAnhHienTai();

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        picAvatar.Image = Image.FromStream(ms);
                    }
                }
                catch
                {
                    picAvatar.Image = null;
                }
            }
            else
            {
                picAvatar.Image = null;
            }
        }

        private void XoaAnhHienTai()
        {
            if (picAvatar.Image != null)
            {
                picAvatar.Image.Dispose();
                picAvatar.Image = null;
            }
        }

        // ==============================================================
        // NÚT CHỌN ẢNH (OPENFILEDIALOG)
        // ==============================================================
        private void btnBrowseAvatar_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current == null)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để gán ảnh đại diện!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh đại diện cho sinh viên";
                ofd.Filter = "Tệp hình ảnh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Tất cả tệp (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string duongDan = ofd.FileName;
                    Student current = (Student)_bindingSource.Current;
                    current.AvatarPath = duongDan;

                    // Đồng bộ lại UI
                    _bindingSource.ResetCurrentItem();
                    LoadAvatarKhongKhoaFile(duongDan);
                }
            }
        }

        // ==============================================================
        // NÚT THÊM SINH VIÊN
        // ==============================================================
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string ma = txtStudentId.Text.Trim();
            string ten = txtFullName.Text.Trim();

            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Mã sinh viên và Họ tên không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_studentList.Any(st => st.StudentId.Equals(ma, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"Mã sinh viên '{ma}' đã tồn tại!", "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtAge.Text.Trim(), out int tuoi) || tuoi <= 0)
            {
                MessageBox.Show("Tuổi phải là số nguyên dương hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string gioiTinh = cboGender.SelectedItem != null ? cboGender.SelectedItem.ToString() : "Nam";
            string anh = txtAvatarPath.Text.Trim();

            Student svMoi = new Student(ma, ten, tuoi, gioiTinh, anh);
            _studentList.Add(svMoi);

            _bindingSource.MoveLast();
            MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==============================================================
        // NÚT XÓA SINH VIÊN
        // ==============================================================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current is Student current)
            {
                DialogResult dr = MessageBox.Show($"Bạn có chắc muốn xóa sinh viên: {current.FullName} ({current.StudentId})?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    _bindingSource.RemoveCurrent();
                    MessageBox.Show("Đã xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==============================================================
        // NÚT CẬP NHẬT / LƯU
        // ==============================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            _bindingSource.EndEdit();
            _bindingSource.ResetBindings(false);
            MessageBox.Show("Đã cập nhật dữ liệu sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==============================================================
        // NÚT XUẤT FILE CSV (SAVEFILEDIALOG + UTF-8)
        // ==============================================================
        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (_studentList.Count == 0)
            {
                MessageBox.Show("Danh sách sinh viên đang trống, không thể xuất file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Chọn nơi lưu tệp CSV";
                sfd.Filter = "Tệp CSV (*.csv)|*.csv|Tất cả tệp (*.*)|*.*";
                sfd.FileName = $"DanhSachSinhVien_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                        {
                            sw.WriteLine("Mã SV,Họ và Tên,Tuổi,Giới tính,Đường dẫn ảnh");

                            foreach (var sv in _studentList)
                            {
                                string tenChuan = sv.FullName.Contains(",") ? $"\"{sv.FullName}\"" : sv.FullName;
                                string pathChuan = sv.AvatarPath.Contains(",") ? $"\"{sv.AvatarPath}\"" : sv.AvatarPath;

                                string line = $"{sv.StudentId},{tenChuan},{sv.Age},{sv.Gender},{pathChuan}";
                                sw.WriteLine(line);
                            }
                        }

                        MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi trong quá trình ghi tệp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Các hàm sự kiện rỗng giữ nguyên để không gây xung đột với Designer
        private void cboGender_SelectedIndexChanged(object sender, EventArgs e) { }
        private void picAvatar_Click(object sender, EventArgs e) { }
        private void txtAvatarPath_TextChanged(object sender, EventArgs e) { }

        private void dgvStudents_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvStudents.ClearSelection();
                dgvStudents.Rows[e.RowIndex].Selected = true;
                // Đồng bộ con trỏ của BindingSource về dòng được click
                _bindingSource.Position = e.RowIndex;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        // 1. Chức năng Xóa dòng
        private void menuContextDelete_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current is Student sv)
            {
                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa sinh viên '{sv.FullName}' ({sv.StudentId})?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _bindingSource.RemoveCurrent();
                    // Gửi thông báo cập nhật lên StatusStrip của Form cha
                    OnThongBaoTrangThai?.Invoke($"Đã xóa sinh viên: {sv.FullName} ({sv.StudentId})");
                }
            }
        }

        // 2. Chức năng Làm mới dữ liệu
        private void menuContextRefresh_Click(object sender, EventArgs e)
        {
            _bindingSource.ResetBindings(false);
            OnThongBaoTrangThai?.Invoke("Đã làm mới dữ liệu danh sách sinh viên.");
        }

        // 3. Chức năng Xem chi tiết
        private void menuContextDetail_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current is Student sv)
            {
                string chiTiet = $"Mã sinh viên: {sv.StudentId}\n" +
                                 $"Họ và tên: {sv.FullName}\n" +
                                 $"Tuổi: {sv.Age}\n" +
                                 $"Giới tính: {sv.Gender}\n" +
                                 $"Đường dẫn ảnh: {sv.AvatarPath}";

                MessageBox.Show(chiTiet, "Chi tiết sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnThongBaoTrangThai?.Invoke($"Đang xem chi tiết sinh viên: {sv.FullName}");
            }
        }

        // ==============================================================
        // SỰ KIỆN BẤM NÚT NẠP DỮ LIỆU BẤT ĐỒNG BỘ (ASYNC)
        // ==============================================================
        private async void btnLoadAsync_Click(object sender, EventArgs e)
        {
            // 1. Tự động khóa nút bấm để tránh kích hoạt trùng lặp
            btnLoadAsync.Enabled = false;

            // Đặt lại giá trị tiến trình về 0%
            prgLoading.Value = 0;
            lblProgressPercent.Text = "0%";
            OnThongBaoTrangThai?.Invoke("Bắt đầu nạp dữ liệu sinh viên trong tiến trình ngầm...");

            // 2. Khởi tạo đối tượng IProgress<int> cập nhật giao diện theo thời gian thực
            var progressReporter = new Progress<int>(percent =>
            {
                // Cập nhật ProgressBar và Label trên Form con
                prgLoading.Value = Math.Min(percent, 100);
                lblProgressPercent.Text = $"{percent}%";

                // Đồng thời đẩy phần trăm tiến trình lên StatusStrip của Form cha MDI
                OnThongBaoTrangThai?.Invoke($"Đang nạp dữ liệu: {percent}%");
            });

            try
            {
                // 3. Chạy tác vụ nạp dữ liệu ngầm mà không làm treo UI
                List<Student> danhSachMoi = await LoadStudentsAsync(progressReporter);

                // Nạp dữ liệu vào BindingList hiện tại
                _studentList.Clear();
                foreach (var sv in danhSachMoi)
                {
                    _studentList.Add(sv);
                }

                _bindingSource.ResetBindings(false);

                // Thông báo hoàn tất
                OnThongBaoTrangThai?.Invoke("Nạp dữ liệu lớn thành công!");
                MessageBox.Show($"Đã nạp thành công {danhSachMoi.Count} sinh viên!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                OnThongBaoTrangThai?.Invoke($"Lỗi nạp dữ liệu: {ex.Message}");
                MessageBox.Show("Đã xảy ra lỗi trong quá trình nạp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 4. Luôn luôn mở lại nút bấm dù thành công hay xảy ra ngoại lệ
                btnLoadAsync.Enabled = true;
            }
        }

        // ==============================================================
        // PHƯƠNG THỨC GIẢ LẬP NẠP DỮ LIỆU NẶNG (TASK.RUN + AWAIT)
        // ==============================================================
        private async Task<List<Student>> LoadStudentsAsync(IProgress<int> progress)
        {
            return await Task.Run(async () =>
            {
                List<Student> list = new List<Student>();
                int tongSoDong = 40; // Giả lập nạp 40 bản ghi

                for (int i = 1; i <= tongSoDong; i++)
                {
                    // Giả lập độ trễ kết nối cơ sở dữ liệu / đọc file (50 mili-giây mỗi dòng)
                    await Task.Delay(50);

                    string ma = $"SV{i:D3}";
                    string ten = (i % 2 == 0) ? $"Lê Văn {i}" : $"Nguyễn Thị {i}";
                    int tuoi = 18 + (i % 5);
                    string gioiTinh = (i % 2 == 0) ? "Nam" : "Nữ";

                    list.Add(new Student(ma, ten, tuoi, gioiTinh, ""));

                    // Tính phần trăm và báo cáo về UI Thread
                    int percent = (int)((double)i / tongSoDong * 100);
                    progress?.Report(percent);
                }

                return list;
            });
        }

        // Các hàm sự kiện click rỗng giữ nguyên để không lỗi Designer
        private void prgLoading_Click(object sender, EventArgs e)
        {

        }

        private void lblProgressPercent_Click(object sender, EventArgs e)
        {

        }

    }

    // ==============================================================
    // LỚP STUDENT ĐẶT Ở DƯỚI CÙNG FILE ĐỂ BẢO VỆ DESIGNER
    // ==============================================================
    public class Student : INotifyPropertyChanged
    {
        private string _studentId;
        private string _fullName;
        private int _age;
        private string _gender;
        private string _avatarPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public string StudentId
        {
            get => _studentId;
            set { _studentId = value; OnPropertyChanged("StudentId"); }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged("FullName"); }
        }

        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged("Age"); }
        }

        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged("Gender"); }
        }

        public string AvatarPath
        {
            get => _avatarPath;
            set { _avatarPath = value; OnPropertyChanged("AvatarPath"); }
        }

        public Student() { }

        public Student(string id, string name, int age, string gender, string avatar)
        {
            _studentId = id;
            _fullName = name;
            _age = age;
            _gender = gender;
            _avatarPath = avatar;
        }

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}