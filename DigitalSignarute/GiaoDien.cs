using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace DigitalSignarute
{
    public partial class GiaoDien : Form
    {
        private RSA rsa = new RSA(); // Khai báo đối tượng và khởi tạo RSA
        public GiaoDien()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void GiaoDien_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtP_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQ_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRandom_Click_Click(object sender, EventArgs e) // Tạo khóa từ giá trị ngẫu nhiên
        {
            rsa = new RSA();
            txtP.Text = rsa.P_test.ToString();
            txtQ.Text = rsa.Q_test.ToString();
            txtN.Text = rsa.N_test.ToString();
            txtPhiN.Text = rsa.Phi_n_test.ToString();
            txtE.Text = rsa.E_test.ToString();
            txtD.Text = rsa.D_test.ToString();

            txtPrivate.Text = string.Format("({0}, {1})", rsa.N_test.ToString(), rsa.E_test.ToString());
            txtPublic.Text = string.Format("({0}, {1})", rsa.N_test.ToString(), rsa.D_test.ToString());
        }

        private void btnTinhToan_Click(object sender, EventArgs e) // Tạo khóa từ giá trị nhập từ bàn phímm
        {
            try
            {
                // Lấy dữ liệu từ TextBox
                int p = int.Parse(txtP.Text);
                int q = int.Parse(txtQ.Text);

                // Tạo đối tượng RSA và kiểm tra số nguyên tố
                RSA rsa = new RSA(p, q);

                // Hiển thị kết quả lên giao diện
                txtN.Text = rsa.N_test.ToString();
                txtPhiN.Text = rsa.Phi_n_test.ToString();
                txtE.Text = rsa.E_test.ToString();
                txtD.Text = rsa.D_test.ToString();

                txtPrivate.Text = string.Format("({0}, {1})", rsa.N_test.ToString(), rsa.E_test.ToString());
                txtPublic.Text = string.Format("({0}, {1})", rsa.N_test.ToString(), rsa.D_test.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void txtE_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void txtNhap_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnKySo_Click(object sender, EventArgs e)
        {
            string inputText = txtNhap.Text; // Lấy dữ liệu từ TextBox đầu vào

            // Băm dữ liệu đầu vào
            string hashedData = rsa.HashData(inputText);

            // Tạo chữ ký số
            string digitalSignature = rsa.taoChuKy(hashedData);

            // Hiển thị giá trị băm
            txtBam.Text = hashedData;

            // Hiển thị chữ ký số
            txtChuKySo.Text = digitalSignature;
        }

        private void btnBamXacMinh_Click(object sender, EventArgs e)
        {
            string inputText = txtNhapXacMinh.Text;

            // Băm dữ liệu cần xác minh
            string hashedDataCheck = rsa.HashData(inputText);

            txtBamXacMinh.Text = hashedDataCheck;
        }

        private void txtKetQuaGiaiMa_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtGiaiMaChuKySo_Click(object sender, EventArgs e)
        {
            string inputText = txtChuKySoXacMinh.Text;

            // Giải mã chữ ký số bằng khóa public
            string digitalSignature = rsa.giaiMaChuKySo(inputText);

            txtKetQuaGiaiMa.Text = digitalSignature;
        }

        private void btnXacMinh_Click(object sender, EventArgs e)
        {
            string inputText = txtNhapXacMinh.Text;
            string digitalSignature = txtChuKySoXacMinh.Text;

            // Băm lại dữ liệu cần xác minh
            string hashedData = rsa.HashData(inputText);

            // Giải mã chữ ký số
            string decryptedHash = rsa.giaiMaChuKySo(digitalSignature);

            // So sánh hai giá trị băm
            if (hashedData == decryptedHash)
            {
                MessageBox.Show("Chữ ký số hợp lệ!");
            }
            else
            {
                MessageBox.Show("Chữ ký số không hợp lệ!");
            }
        }

        private void txtChuKySoXacMinh_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            
        }




        private void txtChuKySo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
