namespace EncryptAndDecrypt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            txtEncript.Text = "ljL5W6X1LU6OLeRSi/Jquam70bLQQKeeWsIJiAnou3e6IQA" +
                "36LS9AL9D/+eV50YfJqfQCrjRc4vU7sKxz+vhE5Z64i944utpjEFaS4cJNjb/oM4VQl5l9ZVM8af5FV5qTHzRD+nN4tw=";
        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOrigin.Text))
                MessageBox.Show("请在文本框中输入待加密文本！");

            txtEncript.Text = DesEncryptHelper.Encrypt("Skyversation.MyColor.EFCore", txtOrigin.Text.Trim());
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEncript.Text))
                MessageBox.Show("请在文本中输入待解密文本");

            txtOrigin.Text = DesEncryptHelper.Decrypt("Skyversation.MyColor.EFCore", txtEncript.Text.Trim());
        }
    }
}