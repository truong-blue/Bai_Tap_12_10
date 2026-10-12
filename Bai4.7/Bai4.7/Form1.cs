using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai4._7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Thiết lập đồng hồ hệ thống chạy ngầm
            CapNhatThoiGian();
            timer1.Interval = 1000;
            timer1.Tick += (s, ev) => CapNhatThoiGian();
            timer1.Start();

            // Hiển thị trạng thái khởi tạo
            lblStatusUser.Text = "Người dùng: Admin";
            CapNhatTrangThai("Hệ thống khởi động thành công.");
        }

        private void CapNhatThoiGian()
        {
            lblStatusTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        // ==============================================================
        // GIAO TIẾP MDI: CẬP NHẬT TRẠNG THÁI TỪ FORM CON LÊN FORM CHA
        // ==============================================================
        public void CapNhatTrangThai(string thongDiep)
        {
            lblStatusMessage.Text = $"[{DateTime.Now:HH:mm:ss}] {thongDiep}";
        }

        // ==============================================================
        // THUẬT TOÁN KIỂM TRA MỞ FORM CON (KHÔNG MỞ TRÙNG LẶP)
        // ==============================================================
        private void MoFormCon(Type formType)
        {
            // 1. Duyệt qua toàn bộ danh sách Form con đang mở
            foreach (Form child in this.MdiChildren)
            {
                if (child.GetType() == formType)
                {
                    // Nếu đã mở thì kích hoạt đưa lên trên cùng và thoát
                    child.Activate();
                    CapNhatTrangThai($"Kích hoạt lại cửa sổ: {child.Text}");
                    return;
                }
            }

            // 2. Nếu chưa mở thì tạo mới và gán làm MdiChild
            try
            {
                Form newChild = (Form)Activator.CreateInstance(formType);
                newChild.MdiParent = this;

                // Nếu là FormStudent, đăng ký nhận sự kiện cập nhật StatusStrip
                if (newChild is FormStudent fs)
                {
                    fs.OnThongBaoTrangThai += CapNhatTrangThai;
                }

                newChild.Show();
                CapNhatTrangThai($"Đã mở cửa sổ: {newChild.Text}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở Form con: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==============================================================
        // SỰ KIỆN CÁC NÚT TRÊN TOOLSTRIP
        // ==============================================================
        private void btnToolSinhVien_Click(object sender, EventArgs e)
        {
            MoFormCon(typeof(FormStudent));
        }

        private void btnToolThoat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // ==============================================================
        // SỰ KIỆN MENU STRIP
        // ==============================================================
        // Menu Hệ thống: Thoát
        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Menu Hệ thống: Đăng xuất
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form child in this.MdiChildren)
            {
                child.Close();
            }
            CapNhatTrangThai("Người dùng đã đăng xuất.");
            MessageBox.Show("Đã đóng tất cả các cửa sổ làm việc!", "Thông báo");
        }

        // Menu Danh mục: Quản lý Sinh viên
        private void quảnLýSinhViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoFormCon(typeof(FormStudent));
        }

        // Menu Danh mục: Quản lý Sản phẩm
        private void quảnLýSảnPhẩmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Quản lý Sản phẩm đang được cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CapNhatTrangThai("Truy cập danh mục Quản lý Sản phẩm");
        }

        // Menu Cửa sổ: Sắp xếp kiểu Cascade
        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
            CapNhatTrangThai("Sắp xếp cửa sổ theo kiểu Cascade.");
        }

        // Menu Cửa sổ: Sắp xếp kiểu Tile Horizontal
        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
            CapNhatTrangThai("Sắp xếp cửa sổ theo kiểu Tile Horizontal.");
        }

        // Menu Cửa sổ: Sắp xếp kiểu Tile Vertical
        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
            CapNhatTrangThai("Sắp xếp cửa sổ theo kiểu Tile Vertical.");
        }

        // ==============================================================
        // CÁC HÀM SỰ KIỆN RỖNG GIỮ LẠI ĐỂ TRÁNH LỖI DESIGNER
        // ==============================================================
        private void cửaSổToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void danhMụcToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void hệThốngToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void lblStatusUser_Click(object sender, EventArgs e) { }
        private void lblStatusMessage_Click(object sender, EventArgs e) { }
        private void lblStatusTime_Click(object sender, EventArgs e) { }
    }
}