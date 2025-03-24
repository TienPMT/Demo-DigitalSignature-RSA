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
            set {
                if (!(kt_SoNguyenTo(value))) // Nếu không phải số nguyên tố, throw ra lỗi -> Catch
                    throw new Exception("p không phải số nguyên tố!");

                p = value;
                TinhToan(); // Cập nhật n và phi(n) mỗi khi p thay đổi
            }
        }

        public int Q_test
        {
            get { return q; }
            set {
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
            
            P_test = random_SoNguyenTo(20, 1000);
            do
            {
                Q_test = random_SoNguyenTo(20, 1000);
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
            int[] danhSachE = { 65537, 3, 5, 17, 257};
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

        // Hàm chuyển string sang byte[] để in màn hình
        public byte[] String_To_Byte(string hex)
        {
            return Enumerable.Range(0, hex.Length / 2)
                             .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                             .ToArray();
        }

        // Hàm chuyển byte[] sang string (hex)
        public string Byte_To_String(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        // "Hàm lấy mod"
        public int RSA_mod(int mx, int ex, int nx)
        {

            //Sử dụng thuật toán "bình phương nhân"
            //Chuyển e sang hệ nhị phân
            int[] a = new int[100];
            int k = 0;
            do
            {
                a[k] = ex % 2;
                k++;
                ex = ex / 2;
            }
            while (ex != 0);
            //Quá trình lấy dư
            int kq = 1;
            for (int i = k - 1; i >= 0; i--)
            {
                kq = (kq * kq) % nx;
                if (a[i] == 1)
                    kq = (kq * mx) % nx;
            }
            return kq;
        }

        // Tạo chữ ký số
        //public string taoChuKy(string dulieubam)
        //{
        //    // Chuyển đổi giá trị băm từ string sang mảng byte
        //    byte[] hashBytes = Convert.FromBase64String(dulieubam);

        //    // Mã hóa giá trị băm bằng khóa riêng d
        //    int[] encryptedHash = new int[hashBytes.Length];
        //    for (int i = 0; i < hashBytes.Length; i++)
        //    {
        //        encryptedHash[i] = RSA_mod(hashBytes[i], D_test, N_test); // mã hóa
        //    }

        //    // Chuyển đổi mảng int thành byte[]
        //    byte[] signatureBytes = new byte[encryptedHash.Length];
        //    for (int i = 0; i < encryptedHash.Length; i++)
        //    {
        //        signatureBytes[i] = (byte)encryptedHash[i];
        //    }

        //    // Trả về chữ ký số dưới dạng Base64
        //    return Convert.ToBase64String(signatureBytes);
        //}

        public string taoChuKy(string dulieubam)
        {
            // Chuyển dữ liệu sang byte Unicode và mã hóa Base64
            byte[] mh_temp1 = Encoding.Unicode.GetBytes(dulieubam);
            string base64 = Convert.ToBase64String(mh_temp1);

            // Mã hóa từng ký tự Base64 bằng RSA
            int[] mh_temp3 = new int[base64.Length];
            for (int i = 0; i < base64.Length; i++)
            {
                int asciiValue = (int)base64[i];
                mh_temp3[i] = RSA_mod(asciiValue, E_test, N_test);
            }

            // Chuyển mảng int sang byte[] và trả về dạng Base64
            byte[] byteResult = new byte[mh_temp3.Length * sizeof(int)];
            Buffer.BlockCopy(mh_temp3, 0, byteResult, 0, byteResult.Length);
            return Convert.ToBase64String(byteResult);
        }

        // Hàm giải mã chữ ký số
        //public string giaiMaChuKy(string ChuKySo)
        //{
        //    // Giải mã chữ ký số bằng khóa công khai
        //    byte[] signatureBytes = Convert.FromBase64String(ChuKySo);
        //    int[] decryptedHash = new int[signatureBytes.Length];
        //    for (int i = 0; i < signatureBytes.Length; i++)
        //    {
        //        decryptedHash[i] = RSA_mod(signatureBytes[i], E_test, N_test); // giải mã
        //    }

        //    // Chuyển đổi mảng int thành byte[]
        //    byte[] decryptedHashBytes = new byte[decryptedHash.Length];
        //    for (int i = 0; i < decryptedHash.Length; i++)
        //    {
        //        decryptedHashBytes[i] = (byte)decryptedHash[i];
        //    }

        //    // Trả về giá trị băm dưới dạng chuỗi Base64
        //    return Convert.ToBase64String(decryptedHashBytes);
        //}
        public string giaiMaChuKy(string ChuKySo)
        {
            // 1. Chuyển chuỗi ký tự thành dữ liệu nhị phân
            byte[] byteMaHoa = Convert.FromBase64String(ChuKySo);

            // 2. Khôi phục mảng số nguyên từ byte[]
            int[] mangMaHoa = new int[byteMaHoa.Length / sizeof(int)];
            Buffer.BlockCopy(byteMaHoa, 0, mangMaHoa, 0, byteMaHoa.Length);

            // 3. Giải mã từng phần tử
            int[] ketQuaGiaiMa = new int[mangMaHoa.Length];
            for (int i = 0; i < mangMaHoa.Length; i++)
            {
                ketQuaGiaiMa[i] = RSA_mod(mangMaHoa[i], D_test, N_test);
            }

            // 4. Chuyển về chuỗi Base64 gốc
            char[] kyTuBase64 = new char[ketQuaGiaiMa.Length];
            for (int i = 0; i < ketQuaGiaiMa.Length; i++)
            {
                kyTuBase64[i] = (char)ketQuaGiaiMa[i];
            }

            // 5. Giải mã dữ liệu cuối cùng
            byte[] duLieuGoc = Convert.FromBase64String(new string(kyTuBase64));
            return Encoding.Unicode.GetString(duLieuGoc);
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
