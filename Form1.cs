using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Image2
{
    public partial class Form1 : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        SqlParameter picture;
        public Form1()
        {
            InitializeComponent();
        }
        private void Open()
        {
            try
            {
                OpenFileDialog f = new OpenFileDialog();
                f.InitialDirectory = @"C:\Users\magom\Pictures";
                f.Filter = "All Files |*.*|JPEGs|*.jpg|Bitmaps|*.bmp|GIFs|*.gif";
                f.FilterIndex = 2;
                if(f.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(f.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.BorderStyle = BorderStyle.Fixed3D;
                    label1.Text = f.SafeFileName.ToString();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void savepicture()
        {
            if(pictureBox1.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                byte[] a = ms.GetBuffer();
                ms.Close();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("picture", a);
                cmd.CommandText = "insert into pictures (name, picture) values ('"+label1.Text.ToString()+"', @picture)";
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
                label1.Text = "";
                pictureBox1.Image = null;
                MessageBox.Show("Image Saved.", "Notification System", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cn.ConnectionString = @"Data Source=DESKTOP-57F7258;Initial Catalog=db0;Integrated Security=True";
            cmd.Connection = cn;
            label1.Text = "";
            picture = new SqlParameter("@picture", SqlDbType.Image);
            loaddata();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            savepicture();
            loaddata();
        }

        private void loaddata()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            cmd.CommandText = "select id,name from pictures";
            cn.Open();
            dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    listBox1.Items.Add(dr[0].ToString());
                    listBox2.Items.Add(dr[1].ToString());
                }
            }
            dr.Close();
            cn.Close();
        }
        private void loadpicture()
        {
            cn.Open();
            cmd.CommandText = "select picture from pictures where id = '" + listBox1.Text.ToString() + "'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            SqlCommandBuilder cbd = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cn.Close();
            byte[] ap = (byte[])(ds.Tables[0].Rows[0]["picture"]);
            MemoryStream ms = new MemoryStream(ap);
            pictureBox1.Image = Image.FromStream(ms);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            label1.Text = listBox2.Text.ToString();
            ms.Close();
        }
        private void ListBox1_Click(object sender, EventArgs e)
        {
            ListBox l = sender as ListBox;
            if(l.SelectedIndex != -1)
            {
                listBox1.SelectedIndex = l.SelectedIndex;
                listBox2.SelectedIndex = l.SelectedIndex;
                loadpicture();
            }
        }
    }
}
