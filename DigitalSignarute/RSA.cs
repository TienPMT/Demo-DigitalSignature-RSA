using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

namespace DigitalSignarute
{
    class RSA
    {
        // ============================================ Thuộc tính, Property ============================================
        int p, q; // 2 số nguyên tố ban đầu
        int n, phi_n;
        int e, d; // để tạo 2 khóa public và private
        private static Random rand = new Random();

        public int P_test
        {
            get { return p; }
            set
            {
                if (!(kt_SoNguyenTo(value))) // Nếu không phải số nguyên tố, throw ra lỗi -> Catch
                    throw new Exception("p không phải số nguyên tố!");

                p = value;
                TinhToan(); // Cập nhật n và phi(n) mỗi khi p thay đổi
            }
        }

        public int Q_test
        {
            get { return q; }
            set
            {
                if (!(kt_SoNguyenTo(value))) // Nếu không phải số nguyên tố, throw ra lỗi -> Catch
                    throw new Exception("q không phải số nguyên tố!");
                if (value == p)
                    throw new Exception("p và q không được bằng nhau!");

                q = value;
                TinhToan(); // Cập nhật n và phi(n) mỗi khi p thay đổi
            }
        }

        public int Phi_n_test
        {
            get { return phi_n; }
            set { phi_n = value; }
        }

        public int N_test
        {
            get { return n; }
            set { n = value; }
        }

        public int D_test
        {
            get { return d; }
            set { d = value; }
        }

        public int E_test
        {
            get { return e; }
            set { e = value; }
        }

        // ============================================ Phương thức khởi tạo (Constructor) ============================================
        // Không tham số (random)
        public RSA()
        {

            P_test = random_SoNguyenTo(20, 100);
            do
            {
                Q_test = random_SoNguyenTo(20, 100);
            } while (P_test == Q_test); // Tránh trường hợp P và Q trùng nhau

            TinhToan(); // Tính 2 giá trị n và phi(n)

            // Tìm 2 giá trị e và d để tạo 2 cặp khóa
            TimE();
            TimD(E_test, Phi_n_test);
        }


        // Có tham số (Người dùng nhập vào p và q)
        public RSA(int p, int q)
        {
            P_test = p;
            Q_test = q;

            TinhToan(); // Tính 2 giá trị n và phi(n)

            // Tìm 2 giá trị e và d để tạo 2 cặp khóa
            TimE();
            TimD(E_test, Phi_n_test);

        }


        // ============================================ Phương thức xử lý ============================================

        // Hàm kiểm tra số nguyên tố
        private bool kt_SoNguyenTo(int n)
        {
            if (n < 2) return false;
            if (n == 2 || n == 3) return true;
            if (n % 2 == 0) return false;

            for (int i = 3; i <= Math.Sqrt(n); i += 2) // i += 2 bởi vì bỏ qua các trường hợp số chẵn, đã được kiểm tra ở trên
            {
                if (n % i == 0) return false;
            }

            return true;
        }

        // Hàm random số nguyên tố
        public int random_SoNguyenTo(int min, int max)
        {
            int result;
            do
            {
                result = rand.Next(min, max);
            } while (!kt_SoNguyenTo(result));

            return result;
        }

        // Tính toán n và phi(n)
        private void TinhToan()
        {
            N_test = P_test * Q_test;
            Phi_n_test = (P_test - 1) * (Q_test - 1);
        }

        // Tìm UCLN
        public int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }


        // Tìm số e hợp lệ
        private void TimE()
        {
            // List các phần tử số nguyên tố thường được chọn làm e
            int[] danhSachE = { 65537, 3, 5, 17, 257 };
            foreach (int n in danhSachE)
            {
                // Tìm e sao cho e là số nguyên tố cùng nhau với Phi_n (có ước chung lớn nhất là 1)
                if (n < Phi_n_test && GCD(n, Phi_n_test) == 1)
                {
                    E_test = n;
                    return;
                }
            }

            // Trường hợp các số nguyên tố bên trên không thỏa điều kiện
            do
            {
                E_test = random_SoNguyenTo(2, Phi_n_test);
            } while (GCD(E_test, Phi_n_test) != 1);
        }

        // Tìm số d hợp lệ
        private void TimD(int e, int phi_n)
        {
            int d, k;
            int gcd = Euclid_MoRong(e, phi_n, out d, out k);

            if (gcd != 1) throw new Exception("Không tìm được d hợp lệ!");

            D_test = (d % phi_n + phi_n) % phi_n; // Gán giá trị d vào D_test
        }

        // Hàm thuật toán Euclid mở rộng để tìm hệ số x, y sao cho a*x + b*y = gcd(a,b)
        private int Euclid_MoRong(int e, int phi_n, out int d, out int k)
        {
            if (phi_n == 0)
            {
                d = 1;
                k = 0;
                return e;
            }

            int d1, k1;
            int gcd = Euclid_MoRong(phi_n, e % phi_n, out d1, out k1);

            d = k1;
            k = d1 - (e / phi_n) * k1;

            return gcd;
        }

        // Hàm băm SHA-256
        public string HashData(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // "Hàm lấy mod"
        public int RSA_mod(int mx, int ex, int nx)
        {
            // Sử dụng thuật toán "bình phương nhân"
            // Chuyển ex sang hệ nhị phân
            int[] a = new int[100];
            int k = 0;
            do
            {
                a[k] = ex % 2;
                k++;
                ex = ex / 2;
            }
            while (ex != 0);

            // Quá trình lấy dư
            int kq = 1;
            for (int i = k - 1; i >= 0; i--)
            {
                kq = (kq * kq) % nx;
                if (a[i] == 1)
                {
                    kq = (kq * mx) % nx;
                }
            }

            // Đảm bảo giá trị trả về nằm trong khoảng cho phép
            if (kq < 0)
            {
                kq += nx;
            }

            return kq;
        }

        public string taoChuKy(string ChuoiVao)
        {

            // Chuyển chuỗi vào thành mảng các ký tự
            int[] mh_temp2 = ChuoiVao.Select(c => (int)c).ToArray();

            // Mảng chứa các ký tự đã mã hóa
            int[] mh_temp3 = new int[mh_temp2.Length];
            for (int i = 0; i < mh_temp2.Length; i++)
            {
                mh_temp3[i] = RSA_mod(mh_temp2[i], D_test, N_test); // mã hóa
            }

            // Chuyển mảng đã mã hóa thành chuỗi
            string str = new string(mh_temp3.Select(c => (char)c).ToArray());

            // Trả về chuỗi đã mã hóa dưới dạng Base64
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        // hàm giải mã
        public string giaiMaChuKy(string ChuoiVao)
        {
            // Giải mã chuỗi từ Base64
            byte[] temp2 = Convert.FromBase64String(ChuoiVao);
            string giaima = Encoding.UTF8.GetString(temp2);

            // Chuyển chuỗi giải mã thành mảng các ký tự
            int[] b = giaima.Select(nd => (int)nd).ToArray();

            // Mảng chứa các ký tự đã giải mã
            int[] c = new int[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                c[i] = RSA_mod(b[i], E_test, N_test); // giải mã
            }

            // Chuyển mảng đã giải mã thành chuỗi
            string str = new string(c.Select(ch => (char)ch).ToArray());

            // Trả về chuỗi đã giải mã
            return str;
        }

        // Hàm xác thực chữ ký số
        public bool xacThucChuKy(string DuLieuXacMinh, string ChuKySo)
        {
            // Băm lại dữ liệu cần xác minh
            string hashedData = HashData(DuLieuXacMinh);

            // Giải mã chữ ký số để lấy giá trị băm gốc
            string decryptedHashString = giaiMaChuKy(ChuKySo);

            // So sánh hai giá trị băm
            return hashedData == decryptedHashString;
        }
    }
}