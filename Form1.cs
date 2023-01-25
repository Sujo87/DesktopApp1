using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;


// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace DesktopApp1
{
    public partial class FormInventura : Form
    {
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\\Inventura.accdb;");
        //Da se izmjene odma vide usred builda:
        //OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\DBs\\Inventura.accdb;");

        OleDbCommand cmnd;
        OleDbDataAdapter dadap;
        DataSet dset;
        OleDbCommandBuilder builder;
        int nRowIndex;
        int rno = 0;
        MemoryStream ms;
        byte[] photo_aray;


        public FormInventura()
        {
            InitializeComponent();
        }

        //10.3. dodana pretraga i po monitoru (izlista odredjeni na svim lokacijama)
        //popravljen richbox i picbox, moraju biti loadani fajlovi al ne crasha vise prog ako nisu
        //promijenjen source IMGs  pa vratit na poslu na stari




        private void FormInventura_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'inventuraDataSet1.ModelNazivi' table. You can move, or remove it, as needed.
            this.modelNaziviTableAdapter1.Fill(this.inventuraDataSet1.ModelNazivi);
            // TODO: This line of code loads data into the 'inventuraDataSet.ModelNazivi' table. You can move, or remove it, as needed.
            this.modelNaziviTableAdapter.Fill(this.inventuraDataSet.ModelNazivi);
            // TODO: This line of code loads data into the 'inventuraDataSet.Prespojnici' table. You can move, or remove it, as needed.
            this.prespojniciTableAdapter.Fill(this.inventuraDataSet.Prespojnici);
            // TODO: This line of code loads data into the 'inventuraDataSet.Skeneri' table. You can move, or remove it, as needed.
            this.skeneriTableAdapter.Fill(this.inventuraDataSet.Skeneri);
            // TODO: This line of code loads data into the 'inventuraDataSet.Printeri' table. You can move, or remove it, as needed.
            this.printeriTableAdapter.Fill(this.inventuraDataSet.Printeri);
            // TODO: This line of code loads data into the 'inventuraDataSet.Monitori' table. You can move, or remove it, as needed.
            this.monitoriTableAdapter.Fill(this.inventuraDataSet.Monitori);
            // TODO: This line of code loads data into the 'inventuraDataSet.Racunala' table. You can move, or remove it, as needed.
            this.racunalaTableAdapter.Fill(this.inventuraDataSet.Racunala);
            // TODO: This line of code loads data into the 'inventuraDataSet.Lokacije' table. You can move, or remove it, as needed.
            this.lokacijeTableAdapter.Fill(this.inventuraDataSet.Lokacije);


            NaLokacijiSearch.Text = "L";


            /*
             *   string sql = "select * from Lokacije";
             *    dset = new DataSet();
            dadap = new OleDbDataAdapter(sql, con);
            builder = new OleDbCommandBuilder(dadap);
            dadap.Fill(dset);
            lokacijeDataGridView.DataSource = dset.Tables[0];
        */


            con.Open();
            OleDbCommand cmd5 = con.CreateCommand();
            cmd5.CommandType = CommandType.Text;
            cmd5.CommandText = "select * from Lokacije";
            cmd5.ExecuteNonQuery();
            DataTable dt5 = new DataTable();
            OleDbDataAdapter da5 = new OleDbDataAdapter(cmd5);
            da5.Fill(dt5);
            lokacijeDataGridView.DataSource = dt5;
            con.Close();



            //puni combobox na zadnjem tabu
            try
            {
                con.Open();
                OleDbCommand command = new OleDbCommand();
                command.Connection = con;

                string query = "select * from ModelNazivi";
                command.CommandText = query;
                OleDbDataReader reader = command.ExecuteReader(); //executenonquery radi samo sa insert, delete i update pa moramo reader koristiti
                while (reader.Read()) //dok ima itema reader cita
                {
                    comboBoxModeli.Items.Add(reader["Model"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error!" + ex);
            }

            //Loada racunala gridview na rac tabu
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = " SELECT Racunala.Barcode, Racunala.NazivRacunala, Racunala.Korisnik, Racunala.Model, Racunala.SerijskiBroj, " +
                "Racunala.Napomena, Racunala.MAC, Racunala.BarcodeLokacije, Racunala.Potvrda, Lokacije.OznakaLokacije,  Lokacije.Prostor," +
                " Lokacije.Kat, Lokacije.Soba FROM (Racunala INNER JOIN Lokacije ON Racunala.BarcodeLokacije = Lokacije.Barcode)";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            racunalaDataGridView.DataSource = dt;
            con.Close();

    

        }

        private void lokacijeBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.lokacijeBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.inventuraDataSet);

        }




        /*
                private Bitmap NewMethod()
                {
                    return new Bitmap(@"\\DBs\\slike\\" + comboBoxModeli.SelectedItem + ".jpg");
                }
                */


        private void btnLokacijeSearch_Click_1(object sender, EventArgs e)
        {
            
            //IZLISTA SVE PRINTERE, SKENERE I PRESPOJNIKE
            /*
           OleDbCommand cmd2 = con.CreateCommand();

           cmd2.CommandType = CommandType.Text;
           cmd2.CommandText = "SELECT Monitori.Barcode, Monitori.Model, Monitori.SerijskiBroj, Prespojnici.Barcode, Prespojnici.Model, " +
               "Prespojnici.SerijskiBroj, Printeri.Barcode, Printeri.Model, Printeri.SerijskiBroj, Skeneri.Barcode, Skeneri.Model," +
               " Skeneri.SerijskiBroj, Skeneri.Napomena, Printeri.Napomena, Prespojnici.Napomena, Monitori.Napomena, Printeri.IPadresa," +
               " Monitori.Potvrda, Prespojnici.Potvrda, Printeri.Potvrda, Skeneri.Potvrda " +
               "FROM(((Monitori INNER JOIN Prespojnici ON Monitori.Barcode = Prespojnici.Barcode) INNER JOIN Printeri ON Monitori.Barcode = Printeri.Barcode)" +
               " INNER JOIN  Skeneri ON Monitori.Barcode = Skeneri.Barcode) where Lokacije.Barcode='"+ textboxLokacijeSearch.Text + "' or Lokacije.Prostor='" + textboxLokacijeSearch.Text + "' or Lokacije.Kat= '" + textboxLokacijeSearch.Text + "' or Lokacije.Soba= '" + textboxLokacijeSearch.Text + "'  ";
           cmd2.ExecuteNonQuery();
           DataTable dt2 = new DataTable();
           OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
           da.Fill(dt2);

           dataGridViewNaUredj.DataSource = dt2;
           */

            //Netreba vise samo treba napuniti textbox na tab1
            /*
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "SELECT [BarcodeLokacije],[Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Monitori] where Monitori.BarcodeLokacije = '" + textboxLokacijeSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Printeri] where Printeri.BarcodeLokacije = '" + textboxLokacijeSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Skeneri] where Skeneri.BarcodeLokacije = '" + textboxLokacijeSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Prespojnici] where Prespojnici.BarcodeLokacije = '" + textboxLokacijeSearch.Text + "' ";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);
            lblNaLok.Text = textboxLokacijeSearch.Text;
            dataGridNaLokOstalo.DataSource = dt2;*/

        }

  

        private void btnRacunalaSearch_Click(object sender, EventArgs e)
        {
       
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT      * FROM Lokacije where Barcode= '" + textboxRacunalaSearch.Text + "'  ";
            cmd.ExecuteNonQuery();
            DataTable dtS = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dtS);
          
            dataGridViewNaLok.DataSource = dtS;
            NaLokacijiSearch.Text = textboxRacunalaNaz.Text;

            OleDbCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "SELECT        Racunala.Barcode, Racunala.NazivRacunala, Racunala.Korisnik, Racunala.Model," +
                " Racunala.SerijskiBroj, Lokacije.OznakaLokacije, Lokacije.Barcode AS BarkodLokacije, Lokacije.Prostor, Lokacije.Kat," +
                " Lokacije.Soba FROM (Racunala INNER JOIN Lokacije ON Racunala.BarcodeLokacije = Lokacije.Barcode)" +
                " where Lokacije.Barcode= '" + textboxRacunalaSearch.Text + "' or Kat= '" + textboxRacunalaSearch.Text + "' or Soba= '" + textboxRacunalaSearch.Text + "'  ";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);

            da2.Fill(dt2);
            dataGridViewRacunalaKorisnik.DataSource = dt2;





            OleDbCommand cmd3 = con.CreateCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "SELECT [BarcodeLokacije],[Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Monitori] where Monitori.BarcodeLokacije = '" + textboxRacunalaNaz.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Printeri] where Printeri.BarcodeLokacije = '" + textboxRacunalaNaz.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Skeneri] where Skeneri.BarcodeLokacije = '" + textboxRacunalaNaz.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Prespojnici] where Prespojnici.BarcodeLokacije = '" + textboxRacunalaNaz.Text + "' ";
            cmd3.ExecuteNonQuery();
            DataTable dt3 = new DataTable();
            OleDbDataAdapter da3 = new OleDbDataAdapter(cmd3);
            da3.Fill(dt3);
            dataGridNaLokOstalo.DataSource = dt3;


            con.Close();



        }



        private void btnRacunalaDelete_Click(object sender, EventArgs e)
        {


            if (MessageBox.Show("Briši raèunalo " + textboxRacunala.Text + "?", "Potvrda brisanja",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {

                try
                {
                    con.Open();
                    OleDbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "delete from Racunala where Barcode ='" + textboxRacunala.Text + "'";

                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Racunalo izbrisano!");

                    textboxRacunalaSearch.Clear();
                    btnRacunalaUpdate.PerformClick();


                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show("Error!" + ex);
                }
            }

        }

        private void btnRacunalaUpdate_Click(object sender, EventArgs e)
        {


            int count = 0;
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = " SELECT Racunala.Barcode, Racunala.NazivRacunala, Racunala.Korisnik, Racunala.Model, Racunala.SerijskiBroj, " +
                "Racunala.Napomena, Racunala.MAC, Racunala.BarcodeLokacije, Racunala.Potvrda, Lokacije.OznakaLokacije,  Lokacije.Prostor," +
                " Lokacije.Kat, Lokacije.Soba FROM (Racunala INNER JOIN Lokacije ON Racunala.BarcodeLokacije = Lokacije.Barcode)";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            count = Convert.ToInt32(dt.Rows.Count.ToString());
            racunalaDataGridView.DataSource = dt;
            con.Close();







            /*
            int count = 0;
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Racunala'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            count = Convert.ToInt32(dt.Rows.Count.ToString());
            racunalaDataGridView.DataSource = dt;
            con.Close();*/
        }

        

        private void btnracunalaConfirmEdit_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                // Potvrda = '"
                // + checkBoxRac.Checked + "',
                string query = "update Racunala set  BarcodeLokacije='"
                + textboxRacunalaBarLok.Text + "', NazivRacunala='"
                + textboxRacunalaNaz.Text + "',  Korisnik='"
                + textboxRacunalaKor.Text + "',  Napomena='"
                + textboxRacunalaNap.Text + "',  Model='"
                + textboxRacunalaMod.Text + "',  SerijskiBroj='"
                + textboxRacunalaSB.Text + "',  MAC='"
                + textboxRacunalaMAC.Text + "',  Potvrda='"
                + textboxRacunalaChecker.Text + "' where Barcode= '"
                + textboxRacunala.Text + "'  ";


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Izmjena informacija uspješna!");



            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error!" + ex);

            }
        }

        private void comboBoxModeli_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxModeli.SelectedIndex > -1)
            {
                try
                {

                    var imageName = string.Format("{0}.jpg", comboBoxModeli.SelectedItem);
                    var file = System.IO.Path.Combine(Application.StartupPath, "Resources", imageName);
                    pictureBoxModeli.Image = Image.FromFile(file);
                }


                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show("Fali Slika za ovaj model!");
                    pictureBoxModeli.Image = null;

                }
            }
        }

        private void btnLokacijeLast_Click(object sender, EventArgs e)
        {
            /* DataSet ds;
             if (ds.Tables[0].Rows.Count > 0)
             {
                 rno = ds.Tables[0].Rows.Count - 1;
                 showdata();
                 MessageBox.Show("Last Record");
             }
             else
                 MessageBox.Show("no records");
                 */
        }

        private void btnLokFirst_Click(object sender, EventArgs e)
        {
            {
                /*
                con.Open();
                DataSet ds = new DataSet(); //stvori novi dataset
        
             
                OleDbCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from Lokacije where Barcode='" + textboxLokacijeSearch.Text + "' or Prostor='"
                    + textboxLokacijeSearch.Text + "' or Kat= '" + textboxLokacijeSearch.Text + "' or Soba= '" + textboxLokacijeSearch.Text + "'  ";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                {

                    DataGridViewRow row = this.lokacijeDataGridView.Rows[0];
                    //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                    //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                    textboxLokacijeSearch.Text = row.Cells[0].Value.ToString();
                 
                 
                    MessageBox.Show("First Record");
                }
                else
                    MessageBox.Show("no records");
                    */
            }
        }

    


        private void racunalaDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnRacunalaDeleteUser_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Oèisti korisnika? " + textboxRacunala.Text + "?", "Potvrda brisanja korisnika",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {

                try
                {
                    textboxRacunalaBarLok.Text = "Lnema";
                    textboxRacunalaKor.Clear();
                    textboxRacunalaOznLok.Clear();
                    textboxRacunalaProstor.Clear();
                    textboxRacunalaKat.Clear();
                    textboxRacunalaSoba.Clear();




                    checkBoxRac.Checked = false;
                    MessageBox.Show("Raèunalo oèišæeno!");
                    btnracunalaConfirmEdit.PerformClick();


                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show("Error!" + ex);
                }
            }
        }


 

        private void modelNaziviDataGridView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnPrinteriSearch_Click(object sender, EventArgs e)
        {
          
            

          
        }

        private void btnPrinteriUpdate_Click(object sender, EventArgs e)
        {
            int count = 0;
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Printeri'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            count = Convert.ToInt32(dt.Rows.Count.ToString());
            printeriDataGridView.DataSource = dt;
            con.Close();
        }

        private void btnPrinteriDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Briši printer " + textboxPrinteriBarcode.Text + "?", "Potvrda brisanja",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {

                try
                {
                    con.Open();
                    OleDbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "delete from Printeri where Barcode ='" + textboxPrinteriBarcode.Text + "'";

                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Printer izbrisan!");

                    textboxPrinteriBarcode.Clear();
                    btnPrinteriUpdate.PerformClick();


                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show("Error!" + ex);
                }
            }
        }

        private void btnPrinteriConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
             
                string query = "update Printeri set SerijskiBroj= '" + textboxPrinteriSrBr.Text + "', Model = '" + textboxPrinteriModel.Text +"'" +
                    ", Napomena = '" + textboxPrinteriNapomena.Text + "', IPadresa= '" + textboxPrinteriIPadresa.Text + "', BarcodeLokacije= '" + textboxPrinteriBarcodeLokacije.Text + "', Potvrda= '" + textboxPrinteriChecker.Text + "' where Barcode = '" + textboxPrinteriBarcode.Text + "'  ";

                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Izmjena informacija uspješna!");
                btnPrinteriUpdate.PerformClick();

             



            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error!" + ex);

            }
        }

        private void checkBoxRac_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRac.Checked == true)
                textboxRacunalaChecker.Text = "-1";
            else textboxRacunalaChecker.Text = "0";

        }

        private void btnSkeneriSearch_Click(object sender, EventArgs e)
        {

          
        }

        private void btnSkeneriUpdate_Click(object sender, EventArgs e)
        {
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Skeneri'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            skeneriDataGridView.DataSource = dt;
            con.Close();
        }

        private void btnSkeneriConfirmEdit_Click(object sender, EventArgs e)
        {

            try
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                // Potvrda = '"
                // + checkBoxRac.Checked + "',
                string query = "update Skeneri set  Model='" + textboxSkeneriModel.Text + "',  SerijskiBroj='" + textboxSkeneriSrBr.Text + "',  Napomena='"  + textboxSkeneriNapomena.Text + "',  BarcodeLokacije='"   + textboxSkeneriBarcodeLokacije.Text + "',  Potvrda='"    + textboxSkeneriChecker.Text + "' where Barcode= '"  + textboxSkeneriBarcode.Text + "'  ";


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Izmjena informacija uspješna!");
                btnSkeneriUpdate.PerformClick();

                textboxSkeneriModel.Clear();
                textboxSkeneriSrBr.Clear();
                textboxSkeneriNapomena.Clear();
                textboxSkeneriBarcodeLokacije.Clear();
                textboxSkeneriBarcode.Clear();


            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error!" + ex);

            }
        }

        private void btnSkeneriDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Briši Skener " + textboxSkeneriBarcode.Text + "?", "Potvrda brisanja",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question,
              MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {

                try
                {
                    con.Open();
                    OleDbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "delete from Skeneri where Barcode ='" + textboxSkeneriBarcode.Text + "'";

                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Skener izbrisan!");

                    textboxSkeneriBarcode.Clear();
                    btnSkeneriUpdate.PerformClick();


                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show("Error!" + ex);
                }
            }
        }

        private void checkBoxScn_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRac.Checked == true)
                textboxSkeneriChecker.Text = "-1";
            else textboxSkeneriChecker.Text = "0";

        }

        private void checkBoxPrnt_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPrnt.Checked == true)
                textboxPrinteriChecker.Text = "-1";
            else textboxPrinteriChecker.Text = "0";
        }

        private void btnMonitoriConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;

                string query = "update Monitori set  Model='"
                + textboxMonitoriModel.Text + "',  SerijskiBroj='"
                + textboxMonitoriSrBr.Text + "',  Napomena='"
                + textboxMonitoriNapomena.Text + "',  Potvrda='"
                + textboxMonitoriChecker.Text + "',  BarcodeLokacije='"
                + textboxMonitoriBarcodeLokacije.Text + "' where Barcode= '"
                + textboxMonitoriBarcode.Text + "'  ";


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Izmjena informacija uspješna!");
                btnMonitoriUpdate.PerformClick();

                textboxPrinteriModel.Clear();
                textboxPrinteriSrBr.Clear();
                textboxPrinteriNapomena.Clear();
                textboxPrinteriIPadresa.Clear();
                textboxPrinteriBarcodeLokacije.Clear();
                textboxPrinteriBarcode.Clear();



            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error!" + ex);

            }
        }

        private void btnMonitoriUpdate_Click(object sender, EventArgs e)
        {
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Monitori'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            monitoriDataGridView.DataSource = dt;
            con.Close();
        }



        private void checkBoxMntr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMntr.Checked == true)
                textboxMonitoriChecker.Text = "-1";
            else textboxMonitoriChecker.Text = "0";
        }

        private void btnMonitoriDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Briši monitor " + textboxPrinteriBarcode.Text + "?", "Potvrda brisanja",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question,
               MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {

                try
                {
                    con.Open();
                    OleDbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "delete from Monitori where Barcode ='" + textboxMonitoriBarcode.Text + "'";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Monitor izbrisan!");
                    textboxMonitoriBarcode.Clear();
                    btnMonitoriUpdate.PerformClick();
                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show("Error!" + ex);
                }
            }
        }

  

        private void rbtnPrint2_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void textboxRacunalaSearch_TextChanged(object sender, EventArgs e) //pisanjem se odma searcha, ako nadje cijeli match za barcode gore ga fokusira u prvi red
        {

            //C/p na prvi tab u search text iz searcha
            NaLokacijiSearch.Text = textboxRacunalaBarLok.Text;

            con.Open();
            //Search na tabu racunala
            OleDbCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = " SELECT Racunala.Barcode, Racunala.NazivRacunala, Racunala.Korisnik, Racunala.Model, Racunala.SerijskiBroj, " +
            "Racunala.Napomena, Racunala.MAC, Racunala.BarcodeLokacije, Racunala.Potvrda, Lokacije.OznakaLokacije,  Lokacije.Prostor," +
            " Lokacije.Kat, Lokacije.Soba FROM (Racunala INNER JOIN Lokacije ON Racunala.BarcodeLokacije = Lokacije.Barcode)" +
            " where Korisnik LIKE '%" + textboxRacunalaSearch.Text + "%' or SerijskiBroj LIKE  '%" + textboxRacunalaSearch.Text + "%'" +
            "or Model LIKE  '%" + textboxRacunalaSearch.Text + "%' or Racunala.Barcode LIKE  '%" + textboxRacunalaSearch.Text + "%'" +
            "or BarcodeLokacije LIKE  '%" + textboxRacunalaSearch.Text + "%' or NazivRacunala LIKE  '%" + textboxRacunalaSearch.Text + "%'";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);
            dataGridViewRacunalaKorisnik.DataSource = dt2;

            /*
            //izlista sva rac na prvom tabu
            OleDbCommand cmd4 = con.CreateCommand();
            cmd4.CommandType = CommandType.Text;
            cmd4.CommandText = "SELECT      * FROM Racunala where BarcodeLokacije= '" + textboxRacunalaBarLok.Text + "'  ";
            cmd4.ExecuteNonQuery();
            DataTable dt4 = new DataTable();
            OleDbDataAdapter da4 = new OleDbDataAdapter(cmd4);
            da4.Fill(dt4);
            dataGridNaLokRac.DataSource = dt4;


            //izlista sve ostale stvari na lokaciji
            OleDbCommand cmd3 = con.CreateCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "SELECT [BarcodeLokacije],[Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Monitori] where Monitori.BarcodeLokacije = '" + textboxRacunalaBarLok.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Printeri] where Printeri.BarcodeLokacije = '" + textboxRacunalaBarLok.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Skeneri] where Skeneri.BarcodeLokacije = '" + textboxRacunalaBarLok.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Prespojnici] where Prespojnici.BarcodeLokacije = '" + textboxRacunalaBarLok.Text + "' ";
            cmd3.ExecuteNonQuery();
            DataTable dt3 = new DataTable();
            OleDbDataAdapter da3 = new OleDbDataAdapter(cmd3);
            da3.Fill(dt3);
            dataGridNaLokOstalo.DataSource = dt3;
            */
            //fokus zadrzava kod sortanja gore
            int indeks;
            foreach (DataGridViewRow row in racunalaDataGridView.Rows)
            {
                if ((string)row.Cells[0].Value == textboxRacunalaSearch.Text)
                {
                    row.Selected = true;
                    indeks = row.Index;
                    racunalaDataGridView.ClearSelection();
                    racunalaDataGridView.Rows[indeks].Selected = true;
                    racunalaDataGridView.FirstDisplayedScrollingRowIndex = indeks;

                }
                else
                {
                    row.Selected = false;
                }
            }
            con.Close();
        }

        private void rbtnRac1_CheckedChanged(object sender, EventArgs e)
        {
            textboxRacunalaSearch.Text = textboxRacunalaNaz.Text;
        }

        private void rbtnRac3_CheckedChanged(object sender, EventArgs e)
        {
            textboxRacunalaSearch.Text = textboxRacunalaNap.Text;
        }

        private void rbtnRac2_CheckedChanged(object sender, EventArgs e)
        {
            textboxRacunalaSearch.Text = textboxRacunala.Text;
        }

        private void rbtnSkeneri2_CheckedChanged(object sender, EventArgs e)
        {
       
        }

        private void rbtnSkeneri1_CheckedChanged(object sender, EventArgs e)
        {
        
        }



        private void btnSwitcheviSearch_Click(object sender, EventArgs e)
        {
         
        }

        private void btnSwitcheviConfirmEdit_Click(object sender, EventArgs e)
        {

            try
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                // Potvrda = '"
                // + checkBoxRac.Checked + "',
                string query = "update Prespojnici set  Model='"
                + textboxSwitchModel.Text + "',  SerijskiBroj='"
                + textboxSwitchSrBr.Text + "',  Napomena='"
                + textboxSwitchNapomena.Text + "',  BarcodeLokacije='"
                + textboxSwBarcodeLokacije.Text + "',  Potvrda='"
                + textBoxSwitchChecker.Text + "' where Barcode= '"
                + textboxSwitchBarcode.Text + "'  ";





                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Izmjena informacija uspješna!");
                btnSwitcheviUpdate.PerformClick();

                textboxSwitchModel.Clear();
                textboxSwitchSrBr.Clear();
                textboxSwitchNapomena.Clear();
                textboxSwBarcodeLokacije.Clear();
                textboxSwitchBarcode.Clear();
                textBoxSwitchChecker.Clear();

            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error!" + ex);

            }
        }

        private void btnSwitcheviDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Briši Switch " + textboxSwitchBarcode.Text + "?", "Potvrda brisanja",
           MessageBoxButtons.YesNo, MessageBoxIcon.Question,
           MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {

                try
                {
                    con.Open();
                    OleDbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "delete from Prespojnici where Barcode ='" + textboxSwitchBarcode.Text + "'";

                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Prespojnik izbrisan!");

                    textboxSwitchBarcode.Clear();
                    btnSwitcheviUpdate.PerformClick();


                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show("Error!" + ex);
                }
            }
        }

        private void btnSwitcheviUpdate_Click(object sender, EventArgs e)
        {
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Prespojnici'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            prespojniciDataGridView.DataSource = dt;
            con.Close();
        }

        private void checkBoxSw_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSw.Checked == true)
                textBoxSwitchChecker.Text = "-1";
            else textBoxSwitchChecker.Text = "0";
        }

     

        private void racunalaDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //this.racunalaDataGridView.Sort(this.dataGridViewTextBoxColumn1, ListSortDirection.Descending);

        }

        private void textboxLokacijeSearch_TextChanged(object sender, EventArgs e)
        {

            //C/p na prvi tab u search text iz searcha
            NaLokacijiSearch.Text = textboxLokacijeSearch.Text;

            //Puni dolje gridview sa svim mogucim lokacijama koje imaju dio iz searcha u barkodu lokacije

            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Lokacije where Barcode='" + textboxLokacijeSearch.Text + "' or Prostor='"
                + textboxLokacijeSearch.Text + "' or Kat= '" + textboxLokacijeSearch.Text + "' or Soba= '" + textboxLokacijeSearch.Text + "'  ";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            dataGridViewNaLok.DataSource = dt;
            con.Close();




            con.Open();
            OleDbCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;


            cmd2.CommandText = " select * from Lokacije where Barcode LIKE '%" + textboxLokacijeSearch.Text + "%' or Prostor LIKE  '%" + textboxLokacijeSearch.Text + "%'" +
                "or Soba LIKE  '%" + textboxLokacijeSearch.Text + "%' or Kat LIKE  '%" + textboxLokacijeSearch.Text + "%' or OznakaLokacije LIKE  '%" + textboxLokacijeSearch.Text + "%' or Prostor LIKE  '%" + textboxLokacijeSearch.Text + "%'";


            // cmd2.CommandText = "SELECT      * FROM Racunala where Korisnik LIKE '%" + textboxRacunalaSearch.Text + "%'  ";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);

            dataGridLok2.DataSource = dt2;
            con.Close();

            int indeks;
            foreach (DataGridViewRow row in lokacijeDataGridView.Rows)
            {
                if ((string)row.Cells[0].Value == NaLokacijiSearch.Text)
                {
                    row.Selected = true;
                    indeks = row.Index;
                    lokacijeDataGridView.ClearSelection();
                    lokacijeDataGridView.Rows[indeks].Selected = true;
                    lokacijeDataGridView.FirstDisplayedScrollingRowIndex = indeks;

                }
                else
                {
                    row.Selected = false;
                }
            }
        }

        private void saveupdateLok_Click(object sender, EventArgs e)
        {

            try
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;

                string query = "update Lokacije set OznakaLokacije= '" + txtNaLokOznLok.Text + "', Prostor= '"+txtNaLokProstor.Text+"', Kat= '"+txtNaLokKat.Text+"', Soba= '"+ txtNaLokSoba.Text+"', Potvrda= '"+ txtNaLokPotvrda.Text+ "' where Barcode = '" + txtNalokBarcode.Text + "'  ";
                //, Prostor = '" + txtNaLokProstor.Text + "', Kat = '" + txtNaLokKat.Text + "', Soba= '" + txtNaLokSoba.Text + "', Potvrda= '" + txtNaLokPotvrda.Text +




                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Izmjena informacija uspješna!");
                btnLokacijeUpdate.PerformClick();

            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error!" + ex);

            }









            /*
            try
            {
                dadap.Update(dset.Tables[0]);
                MessageBox.Show("updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */


        }

        private void lokacijedelete_Click(object sender, EventArgs e)
        {
            con.Open();
            OleDbCommand cmd5 = con.CreateCommand();
            cmd5.CommandType = CommandType.Text;
            cmd5.CommandText = "select * from Lokacije";
            cmd5.ExecuteNonQuery();
            DataTable dt5 = new DataTable();
            OleDbDataAdapter da5 = new OleDbDataAdapter(cmd5);
            da5.Fill(dt5);
            lokacijeDataGridView.DataSource = dt5;
            con.Close();
        }

        private void racunalaDataGridView_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e)
        {




            // int rc = racunalaDataGridView.CurrentCell.RowIndex;
            //racunalaDataGridView.Rows[rc].Selected = true;
            //   racunalaDataGridView.CurrentCell = racunalaDataGridView.Rows[nRowIndex].Cells[0];
            //  MessageBox.Show(nRowIndex.ToString());

            /*
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Racunala'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            int saverow = 0;
            if (racunalaDataGridView.Rows.Count > 0 && racunalaDataGridView.FirstDisplayedCell != null)
                saverow = racunalaDataGridView.FirstDisplayedCell.RowIndex;

            racunalaDataGridView.DataSource = racunalaDataGridView.DataSource = dt;

            if (saverow != 0 && saverow < racunalaDataGridView.Rows.Count)
                racunalaDataGridView.FirstDisplayedScrollingColumnIndex = saverow;


            con.Close();
            */


        }

        private void racunalaDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.racunalaDataGridView.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                racunalaDataGridView.Rows[e.RowIndex].Selected = true;


            }
        }

        private void dataGridViewRacunalaKorisnik_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridViewRacunalaKorisnik.Rows[e.RowIndex];
               



                textboxRacunala.Text = row.Cells[0].Value.ToString();
                textboxRacunalaNaz.Text = row.Cells[1].Value.ToString();
                textboxRacunalaKor.Text = row.Cells[2].Value.ToString();
                textboxRacunalaMod.Text = row.Cells[3].Value.ToString();
                textboxRacunalaSB.Text = row.Cells[4].Value.ToString();
                textboxRacunalaNap.Text = row.Cells[5].Value.ToString();
                textboxRacunalaMAC.Text = row.Cells[6].Value.ToString();
                textboxRacunalaBarLok.Text = row.Cells[7].Value.ToString();
                textboxRacunalaOznLok.Text = row.Cells[9].Value.ToString();
                textboxRacunalaProstor.Text = row.Cells[10].Value.ToString();
                textboxRacunalaKat.Text = row.Cells[11].Value.ToString();
                textboxRacunalaSoba.Text = row.Cells[12].Value.ToString();
                DataGridViewCheckBoxCell chk = row.Cells[8] as DataGridViewCheckBoxCell;
                textboxRacunalaSearch.Text = row.Cells[0].Value.ToString();


                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxRac.Checked = true;
                }
                else checkBoxRac.Checked = false;

            }
        }



        // int rc = racunalaDataGridView.CurrentCell.RowIndex;
        //racunalaDataGridView.Rows[rc].Selected = true;
        //racunalaDataGridView.CurrentCell = racunalaDataGridView.Rows[nRowIndex].Cells[0];
        //  MessageBox.Show(nRowIndex.ToString());

        public string typedChars = string.Empty;



        private void racunalaDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {/*
            if (e.RowIndex==-1)
            {
                trazenjerac = racunalaDataGridView.SelectedRows[1].Cells["Barcode"].Value.ToString();
            }
            */
        }

        private void racunalaDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            //MANUAL SORT CODE + data binding

            DataGridViewColumn newColumn = racunalaDataGridView.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = racunalaDataGridView.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                   racunalaDataGridView.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            racunalaDataGridView.Sort(newColumn, direction);
            newColumn.HeaderCell.SortGlyphDirection =
                direction == ListSortDirection.Ascending ?
                SortOrder.Ascending : SortOrder.Descending;
            // textboxRacunalaSearch.Text += " ";

            //forcea da proradi textchange pa da fokusira ponovno row nakon sorta, potreban poseban sort a ne defaultni
            String temp = textboxRacunalaSearch.Text.ToString();
            textboxRacunalaSearch.Clear();
            textboxRacunalaSearch.Text = temp.ToString();
        }

        private void racunalaDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Put each of the columns into programmatic sort mode.
            foreach (DataGridViewColumn column in racunalaDataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Programmatic;
            }

        }

        private void NaLokacijiSearch_TextChanged(object sender, EventArgs e)
        {
            //puni lokacije gridview na prvom tabu
            con.Close();
            con.Open();
            OleDbCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "SELECT      * FROM Lokacije where Barcode LIKE '%" + NaLokacijiSearch.Text + "%'  ";

            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);
            dataGridViewNaLok.DataSource = dt2;
            con.Close();


            con.Open();
            //izlista sva rac na prvom tabu
            OleDbCommand cmd4 = con.CreateCommand();
            cmd4.CommandType = CommandType.Text;
            cmd4.CommandText = "SELECT      * FROM Racunala where BarcodeLokacije= '" + NaLokacijiSearch.Text + "'  ";
            cmd4.ExecuteNonQuery();
            DataTable dt4 = new DataTable();
            OleDbDataAdapter da4 = new OleDbDataAdapter(cmd4);
            da4.Fill(dt4);
            dataGridNaLokRac.DataSource = dt4;
            con.Close();


            con.Open();

            //izlista sve ostale stvari na lokaciji
            OleDbCommand cmd3 = con.CreateCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "SELECT [BarcodeLokacije],[Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Monitori] where Monitori.BarcodeLokacije = '" + NaLokacijiSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Printeri] where Printeri.BarcodeLokacije = '" + NaLokacijiSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Skeneri] where Skeneri.BarcodeLokacije = '" + NaLokacijiSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Prespojnici] where Prespojnici.BarcodeLokacije = '" + NaLokacijiSearch.Text + "' ";
            cmd3.ExecuteNonQuery();
            DataTable dt3 = new DataTable();
            OleDbDataAdapter da3 = new OleDbDataAdapter(cmd3);
            da3.Fill(dt3);
            dataGridNaLokOstalo.DataSource = dt3;
            con.Close();

            int indeks;
            foreach (DataGridViewRow row in dataGridViewNaLok.Rows)
            {
                if ((string)row.Cells[0].Value == NaLokacijiSearch.Text)
                {
                    row.Selected = true;
                    indeks = row.Index;
                    dataGridViewNaLok.ClearSelection();
                    dataGridViewNaLok.Rows[indeks].Selected = true;
                    dataGridViewNaLok.FirstDisplayedScrollingRowIndex = indeks;

                }
                else
                {
                    row.Selected = false;
                }

            }
            /*
    foreach (DataGridViewRow row in racunalaDataGridView.Rows)
    {
    if (textboxRacunala.Text != null && textboxRacunala.Text == row.Cells[0].Value.ToString() )
    {
    indexrow = row.Index;
    break;
    }

    racunalaDataGridView.Rows[indexrow].Selected = true;
    }
    */

        }

        private void dataGridViewNaLok_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridViewNaLok.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                NaLokacijiSearch.Text = row.Cells[0].Value.ToString();
            }
        }

        private void dataGridViewNaLok_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn newColumn = dataGridViewNaLok.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = dataGridViewNaLok.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                   dataGridViewNaLok.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            dataGridViewNaLok.Sort(newColumn, direction);
            newColumn.HeaderCell.SortGlyphDirection =
                direction == ListSortDirection.Ascending ?
                SortOrder.Ascending : SortOrder.Descending;
            

            //forcea da proradi textchange pa da fokusira ponovno row nakon sorta, potreban poseban sort napraviti u eventima sa columnheadermouseclick a ne defaultni
            String temp = textboxRacunalaSearch.Text.ToString();
            NaLokacijiSearch.Clear();
            NaLokacijiSearch.Text = temp.ToString();
        }

        private void lokacijeDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.lokacijeDataGridView.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxLokacijeSearch.Text = row.Cells[0].Value.ToString();
                txtNalokBarcode.Text= row.Cells[0].Value.ToString();
                txtNaLokOznLok.Text= row.Cells[1].Value.ToString();
                txtNaLokProstor.Text = row.Cells[2].Value.ToString();
                txtNaLokKat.Text = row.Cells[3].Value.ToString();
                txtNaLokSoba.Text = row.Cells[4].Value.ToString();
              //  txtNaLokPotvrda.Text = row.Cells[5].Value.ToString();

                DataGridViewCheckBoxCell chk = row.Cells[5] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxLok.Checked = true;
                }
                else checkBoxLok.Checked = false;

            }
        }

        private void dataGridLok2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridLok2.Rows[e.RowIndex];
                textboxLokacijeSearch.Text = row.Cells[0].Value.ToString();
                
                
            }




        }

        private void lokacijeDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MANUAL SORT CODE + data binding

            DataGridViewColumn newColumn = lokacijeDataGridView.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = lokacijeDataGridView.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                   lokacijeDataGridView.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

      
            // textboxRacunalaSearch.Text += " ";

            try
            {
                lokacijeDataGridView.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
            catch (Exception ex)
            {
               
            }


            //forcea da proradi textchange pa da fokusira ponovno row nakon sorta, potreban poseban sort a ne defaultni
            String temp = textboxLokacijeSearch.Text.ToString();
            textboxLokacijeSearch.Clear();
            textboxLokacijeSearch.Text = temp.ToString();
        }

        private void dataGridNaLokRac_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridNaLokRac_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridNaLokRac.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxRacunalaSearch.Text = row.Cells[0].Value.ToString();
                tabControlSve.SelectedTab = tabPgRacunala;
                textboxRacunalaNaz.Clear();
                textboxRacunala.Clear();
                textboxRacunalaBarLok.Clear();
                textboxRacunalaKor.Clear();
                textboxRacunalaMod.Clear();
                textboxRacunalaSB.Clear();
                textboxRacunalaNap.Clear();
                textboxRacunalaMAC.Clear();
                textboxRacunalaBarLok.Clear();
                checkBoxRac.Checked = false;

                textboxRacunalaProstor.Clear();
                textboxRacunalaKat.Clear();
                textboxRacunalaSoba.Clear();
                textboxRacunalaOznLok.Clear();
            }

        }
  
        private void textboxMonitoriSearch_TextChanged(object sender, EventArgs e)
        {
          
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT      * FROM Lokacije where Barcode LIKE '%" + textboxMonitoriSearch.Text+"%'";
            cmd.ExecuteNonQuery();
            DataTable dtM = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dtM);
            
            dataGridViewNaLok.DataSource = dtM;

            con.Close();

            NaLokacijiSearch.Text = textboxMonitoriBarcodeLokacije.Text;
            con.Open();
            OleDbCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "SELECT        Monitori.Barcode,  Monitori.Model, Monitori.SerijskiBroj, Monitori.Napomena, Monitori.Potvrda, " +
                " Lokacije.Barcode AS BarkodLokacije, Lokacije.OznakaLokacije,  Lokacije.Prostor, Lokacije.Kat," +
                " Lokacije.Soba FROM (Monitori INNER JOIN Lokacije ON Monitori.BarcodeLokacije = Lokacije.Barcode)" +
                " where Monitori.Barcode LIKE '%" + textboxMonitoriSearch.Text + "%' or Monitori.Model LIKE '%" + textboxMonitoriSearch.Text + "%' or Monitori.SerijskiBroj LIKE  '%" + textboxMonitoriSearch.Text + "%'or Monitori.BarcodeLokacije LIKE '%" + textboxMonitoriSearch.Text + "%'  ";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);
            dataGridViewMonitoriKorisnik.DataSource = dt2;


            con.Close();

    
            con.Open();


            OleDbCommand cmd3 = con.CreateCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "SELECT [BarcodeLokacije],[Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Monitori] where Monitori.BarcodeLokacije = '" + textboxMonitoriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Printeri] where Printeri.BarcodeLokacije = '" + textboxMonitoriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Skeneri] where Skeneri.BarcodeLokacije = '" + textboxMonitoriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Prespojnici] where Prespojnici.BarcodeLokacije = '" + textboxMonitoriSearch.Text + "' ";
            cmd3.ExecuteNonQuery();
            DataTable dt3 = new DataTable();
            OleDbDataAdapter da3 = new OleDbDataAdapter(cmd3);
            da3.Fill(dt3);
            dataGridNaLokOstalo.DataSource = dt3;
           
            con.Close();
            con.Open();
            int indeks;
            foreach (DataGridViewRow row in monitoriDataGridView.Rows)
            {
                if ((string)row.Cells[0].Value == textboxMonitoriSearch.Text)
                {
                    row.Selected = true;
                    indeks = row.Index;
                    monitoriDataGridView.ClearSelection();
                    monitoriDataGridView.Rows[indeks].Selected = true;
                    monitoriDataGridView.FirstDisplayedScrollingRowIndex = indeks;

                }
                else
                {
                    row.Selected = false;
                }
            }
            con.Close();

        }

        private void dataGridViewMonitoriKorisnik_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridViewMonitoriKorisnik.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
               
                textboxMonitoriBarcode.Text = row.Cells[0].Value.ToString();
                textboxMonitoriModel.Text = row.Cells[1].Value.ToString();
                textboxMonitoriSrBr.Text = row.Cells[2].Value.ToString();
                textboxMonitoriNapomena.Text = row.Cells[3].Value.ToString();
                textboxMonitoriBarcodeLokacije.Text = row.Cells[5].Value.ToString();
                textboxMonitoriSearch.Text = row.Cells[0].Value.ToString();
                DataGridViewCheckBoxCell chk = row.Cells[4] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxMntr.Checked = true;
                }
                else checkBoxMntr.Checked = false;

            }
        }

        private void textboxPrinteriSearch_TextChanged(object sender, EventArgs e)
        {
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT      * FROM Lokacije where Barcode= '" + textboxPrinteriSearch.Text + "'  ";
            cmd.ExecuteNonQuery();
            DataTable dtM = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dtM);

            dataGridViewNaLok.DataSource = dtM;
            con.Close();

            NaLokacijiSearch.Text = textboxPrinteriBarcodeLokacije.Text;

            con.Open();
            OleDbCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "SELECT        Printeri.Barcode,  Printeri.Model, Printeri.SerijskiBroj, Printeri.IPadresa, Printeri.Potvrda, Printeri.Napomena," +
                " Lokacije.OznakaLokacije, Lokacije.Barcode AS BarkodLokacije, Lokacije.Prostor, Lokacije.Kat," +
                " Lokacije.Soba FROM (Printeri INNER JOIN Lokacije ON Printeri.BarcodeLokacije = Lokacije.Barcode)" +
                " where Printeri.Barcode='" + textboxPrinteriSearch.Text + "'or Printeri.IPadresa='" + textboxPrinteriSearch.Text + "' or Printeri.Model= '" + textboxPrinteriSearch.Text + "' or Printeri.SerijskiBroj= '" + textboxPrinteriSearch.Text + "'or Printeri.BarcodeLokacije= '" + textboxPrinteriSearch.Text + "'  ";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);
            dataGridViewPrinteriKorisnik.DataSource = dt2;
            con.Close();

            con.Open();
            OleDbCommand cmd3 = con.CreateCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "SELECT [BarcodeLokacije],[Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Monitori] where Monitori.BarcodeLokacije = '" + textboxPrinteriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Printeri] where Printeri.BarcodeLokacije = '" + textboxPrinteriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Skeneri] where Skeneri.BarcodeLokacije = '" + textboxPrinteriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Prespojnici] where Prespojnici.BarcodeLokacije = '" + textboxPrinteriSearch.Text + "' ";

            cmd3.ExecuteNonQuery();
            DataTable dt3 = new DataTable();
            OleDbDataAdapter da3 = new OleDbDataAdapter(cmd3);
            da3.Fill(dt3);
            dataGridNaLokOstalo.DataSource = dt3;
            con.Close();

            con.Open();
            int indeks;
            foreach (DataGridViewRow row in printeriDataGridView.Rows)
            {
                if ((string)row.Cells[0].Value == textboxPrinteriSearch.Text)
                {
                    row.Selected = true;
                    indeks = row.Index;
                    printeriDataGridView.ClearSelection();
                    printeriDataGridView.Rows[indeks].Selected = true;
                    printeriDataGridView.FirstDisplayedScrollingRowIndex = indeks;

                }
                else
                {
                    row.Selected = false;
                }
            }
            con.Close();
        }

        private void monitoriDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MANUAL SORT CODE + data binding

            DataGridViewColumn newColumn = monitoriDataGridView.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = monitoriDataGridView.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                   monitoriDataGridView.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

       
            // textboxRacunalaSearch.Text += " ";

            try
            {
                monitoriDataGridView.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
            catch (Exception ex)
            {

            }





            //forcea da proradi textchange pa da fokusira ponovno row nakon sorta, potreban poseban sort a ne defaultni
            String temp = textboxMonitoriSearch.Text.ToString();
            textboxMonitoriSearch.Clear();
            textboxMonitoriSearch.Text = temp.ToString();
        }


        private void printeriDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MANUAL SORT CODE + data binding

            DataGridViewColumn newColumn = printeriDataGridView.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = printeriDataGridView.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                   printeriDataGridView.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }



            try
            {
                printeriDataGridView.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
            catch (Exception ex)
            {

            }

            //forcea da proradi textchange pa da fokusira ponovno row nakon sorta, potreban poseban sort a ne defaultni
            String temp = textboxPrinteriSearch.Text.ToString();
            textboxPrinteriSearch.Clear();
            textboxPrinteriSearch.Text = temp.ToString();
        }

        private void dataGridViewPrinteriKorisnik_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridViewPrinteriKorisnik.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxPrinteriBarcode.Text = row.Cells[0].Value.ToString();
                textboxPrinteriModel.Text = row.Cells[1].Value.ToString();
                textboxPrinteriSrBr.Text = row.Cells[2].Value.ToString();
                textboxPrinteriNapomena.Text = row.Cells[5].Value.ToString();
                textboxPrinteriIPadresa.Text = row.Cells[3].Value.ToString();
                textboxPrinteriBarcodeLokacije.Text = row.Cells[7].Value.ToString();
                textboxPrinteriSearch.Text = row.Cells[0].Value.ToString();

                DataGridViewCheckBoxCell chk = row.Cells[4] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxPrnt.Checked = true;
                }
                else checkBoxPrnt.Checked = false;


            }

        }

        private void skeneriDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MANUAL SORT CODE + data binding

            DataGridViewColumn newColumn = skeneriDataGridView.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = skeneriDataGridView.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                    skeneriDataGridView.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }



            try
            {
                skeneriDataGridView.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
            catch (Exception ex)
            {

            }

            //forcea da proradi textchange pa da fokusira ponovno row nakon sorta, potreban poseban sort a ne defaultni
            String temp = textboxSkeneriSearch.Text.ToString();
            textboxSkeneriSearch.Clear();
            textboxSkeneriSearch.Text = temp.ToString();
        }
            private void dataGridViewSkeneriKorisnik_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridViewSkeneriKorisnik.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxSkeneriBarcode.Text = row.Cells[0].Value.ToString();
                textboxSkeneriModel.Text = row.Cells[1].Value.ToString();
                textboxSkeneriSrBr.Text = row.Cells[2].Value.ToString();
                textboxSkeneriNapomena.Text = row.Cells[3].Value.ToString();
                textboxSkeneriBarcodeLokacije.Text = row.Cells[6].Value.ToString();
                textboxSkeneriSearch.Text = row.Cells[0].Value.ToString();

              


                DataGridViewCheckBoxCell chk = row.Cells[4] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxScn.Checked = true;
                }
                else checkBoxScn.Checked = false;



            }
        }

        private void textboxSkeneriSearch_TextChanged(object sender, EventArgs e)
        {
       
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT      * FROM Lokacije where Barcode= '" + textboxSkeneriSearch.Text + "'  ";
            cmd.ExecuteNonQuery();
            DataTable dtS = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dtS);
            dataGridViewNaLok.DataSource = dtS;
            con.Close();
            NaLokacijiSearch.Text = textboxSkeneriBarcodeLokacije.Text;
            con.Open();
            OleDbCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "SELECT        Skeneri.Barcode,  Skeneri.Model, Skeneri.SerijskiBroj, Skeneri.Napomena, Skeneri.Potvrda,  " +
                " Lokacije.OznakaLokacije, Lokacije.Barcode AS BarkodLokacije, Lokacije.Prostor, Lokacije.Kat," +
                " Lokacije.Soba FROM (Skeneri INNER JOIN Lokacije ON Skeneri.BarcodeLokacije = Lokacije.Barcode)" +
                " where Skeneri.Barcode LIKE '%" + textboxSkeneriSearch.Text + "%' or Skeneri.Model LIKE '%" + textboxSkeneriSearch.Text + "%' or Skeneri.SerijskiBroj LIKE '%" + textboxSkeneriSearch.Text + "%'or Skeneri.BarcodeLokacije LIKE '%" + textboxSkeneriSearch.Text + "%'  ";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);
            dataGridViewSkeneriKorisnik.DataSource = dt2;
            con.Close();

            con.Open();
            OleDbCommand cmd3 = con.CreateCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "SELECT [BarcodeLokacije],[Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Monitori] where Monitori.BarcodeLokacije = '" + textboxSkeneriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Printeri] where Printeri.BarcodeLokacije = '" + textboxSkeneriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Skeneri] where Skeneri.BarcodeLokacije = '" + textboxSkeneriSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Prespojnici] where Prespojnici.BarcodeLokacije = '" + textboxSkeneriSearch.Text + "' ";

            cmd3.ExecuteNonQuery();
            DataTable dt3 = new DataTable();
            OleDbDataAdapter da3 = new OleDbDataAdapter(cmd3);
            da3.Fill(dt3);
            dataGridNaLokOstalo.DataSource = dt3;

            con.Close();

            con.Open();
            int indeks;
            foreach (DataGridViewRow row in skeneriDataGridView.Rows)
            {
                if ((string)row.Cells[0].Value == textboxSkeneriSearch.Text)
                {
                    row.Selected = true;
                    indeks = row.Index;
                    skeneriDataGridView.ClearSelection();
                    skeneriDataGridView.Rows[indeks].Selected = true;
                    skeneriDataGridView.FirstDisplayedScrollingRowIndex = indeks;

                }
                else
                {
                    row.Selected = false;
                }
            }
            con.Close();


        }

        private void textboxSwitchSearch_TextChanged(object sender, EventArgs e)
        {
       
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT      * FROM Lokacije where Barcode= '" + textboxSwitchSearch.Text + "'  ";
            cmd.ExecuteNonQuery();
            DataTable dtS = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dtS);

            dataGridViewNaLok.DataSource = dtS;
            con.Close();
            NaLokacijiSearch.Text = textboxSwBarcodeLokacije.Text;
            con.Open();
            OleDbCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "SELECT        Prespojnici.Barcode,  Prespojnici.Model, Prespojnici.SerijskiBroj, Prespojnici.Napomena, Prespojnici.Potvrda,  " +
                " Lokacije.OznakaLokacije, Lokacije.Barcode AS BarkodLokacije, Lokacije.Prostor, Lokacije.Kat," +
                " Lokacije.Soba FROM (Prespojnici INNER JOIN Lokacije ON Prespojnici.BarcodeLokacije = Lokacije.Barcode)" +
                " where Prespojnici.Barcode LIKE '%" + textboxSwitchSearch.Text + "%' or Prespojnici.Model LIKE '%" + textboxSwitchSearch.Text + "%' or Prespojnici.SerijskiBroj LIKE '%" + textboxSwitchSearch.Text + "%' or Prespojnici.BarcodeLokacije LIKE '%" + textboxSwitchSearch.Text + "%'  ";
            cmd2.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);



            dataGridViewSwitcheviKorisnik.DataSource = dt2;

            OleDbCommand cmd3 = con.CreateCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "SELECT [BarcodeLokacije],[Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Monitori] where Monitori.BarcodeLokacije = '" + textboxSwitchSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Printeri] where Printeri.BarcodeLokacije = '" + textboxSwitchSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Skeneri] where Skeneri.BarcodeLokacije = '" + textboxSwitchSearch.Text + "' " +
                "UNION SELECT [BarcodeLokacije], [Barcode], [Model], [SerijskiBroj], [Napomena] FROM [Prespojnici] where Prespojnici.BarcodeLokacije = '" + textboxSwitchSearch.Text + "' ";

            cmd3.ExecuteNonQuery();
            DataTable dt3 = new DataTable();
            OleDbDataAdapter da3 = new OleDbDataAdapter(cmd3);
            da3.Fill(dt3);
            dataGridNaLokOstalo.DataSource = dt3;
            con.Close();

            con.Open();
            int indeks;
            foreach (DataGridViewRow row in prespojniciDataGridView.Rows)
            {
                if ((string)row.Cells[0].Value == textboxSwitchSearch.Text)
                {
                    row.Selected = true;
                    indeks = row.Index;
                    prespojniciDataGridView.ClearSelection();
                    prespojniciDataGridView.Rows[indeks].Selected = true;
                    prespojniciDataGridView.FirstDisplayedScrollingRowIndex = indeks;

                }
                else
                {
                    row.Selected = false;
                }
            }
            con.Close();


        }

           private void prespojniciDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MANUAL SORT CODE + data binding

            DataGridViewColumn newColumn = prespojniciDataGridView.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = prespojniciDataGridView.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                    prespojniciDataGridView.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }



            try
            {
                prespojniciDataGridView.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
            catch (Exception ex)
            {

            }

            //forcea da proradi textchange pa da fokusira ponovno row nakon sorta, potreban poseban sort a ne defaultni
            String temp = textboxSwitchSearch.Text.ToString();
            textboxSwitchSearch.Clear();
            textboxSwitchSearch.Text = temp.ToString();
        }

        private void dataGridViewSwitcheviKorisnik_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridViewSwitcheviKorisnik.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxSwitchBarcode.Text = row.Cells[0].Value.ToString();
                textboxSwitchModel.Text = row.Cells[1].Value.ToString();
                textboxSwitchSrBr.Text = row.Cells[2].Value.ToString();
                textboxSwitchNapomena.Text = row.Cells[3].Value.ToString();
                textboxSwBarcodeLokacije.Text = row.Cells[6].Value.ToString();



                textboxSwitchSearch.Text = row.Cells[0].Value.ToString();


                DataGridViewCheckBoxCell chk = row.Cells[4] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxSw.Checked = true;
                }
                else checkBoxSw.Checked = false;
            }
        }

        private void modelNaziviDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.modelNaziviDataGridView.Rows[e.RowIndex];
                //combobox selektirani item postaje NazivProizvoda selektirano proizvoda
                comboBoxModeli.SelectedItem = row.Cells[0].Value.ToString();
                richTextBox1.Text = row.Cells[1].Value.ToString();

            }
            /*

            byte[] ImageByte = null;
            MemoryStream MemStream = null;
            PictureBox PicBx = new PictureBox();
            object OB;

            string WorkingDirectory = Application.StartupPath + "\\";
            
            con.Open();
            int ImageID = 6;
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT Slika FROM ModelNazivi WHERE Model = " + comboBoxModeli.SelectedItem + "";
            cmd.ExecuteNonQuery();
            
            ImageByte = cmd.ExecuteScalar();
            MemStream = new MemoryStream(ImageByte);
            PicBx.Image = Image.FromStream(MemStream);

           
            
            sqlCommand = "SELECT ImageObject FROM ImagesTable WHERE ImageID = " + ImageID + "";
            comm = new OleDbCommand(sqlCommand, cnction);
            ImageByte = comm.ExecuteScalar();
            MemStream = new MemoryStream(ImageByte);
            PicBx.Image = Image.FromStream(MemStream);*/

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "jpeg|*.jpg|bmp|*.bmp|all files|*.*";
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText ="insert into student(Model, Slika, Napomena) values(" + textBox1.Text + "',@photo,'" + textBox2.Text + " )";
            conv_photo();
            con.Open();
             cmd.ExecuteNonQuery();
            con.Close();
          
        }
        void conv_photo()
        {
            //converting photo to binary data
            if (pictureBox1.Image != null)
            {
                //using MemoryStream:
                ms = new MemoryStream();
                pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                byte[] photo_aray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(photo_aray, 0, photo_aray.Length);
                cmnd.Parameters.AddWithValue("@photo", photo_aray);
            }
        }

        private void textboxRacunalaMod_MouseHover(object sender, EventArgs e)
        {

        }

        private void btnLokacijeDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Briši lokaciju " + textboxRacunala.Text + "? Don't do it", "Potvrda brisanja",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {

                try
                {
                    con.Open();
                    OleDbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "delete from Lokacije where Barcode ='" + txtNalokBarcode.Text + "'";

                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Lokacija izbrisana!");

                    txtNalokBarcode.Text = txtNaLokKat.Text = txtNaLokOznLok.Text = txtNaLokPotvrda.Text = txtNaLokProstor.Text = txtNaLokSoba.Text = "";
                    btnLokacijeUpdate.PerformClick();


                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show("Error!" + ex);
                }
            }
        }

        private void checkBoxLok_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLok.Checked == true)
                txtNaLokPotvrda.Text = "-1";
            else txtNaLokPotvrda.Text = "0";
        }

        private void dataGridNaLokOstalo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
               //double click prebacuje na odredjeni tab
      
                DataGridViewRow row = this.dataGridNaLokOstalo.Rows[e.RowIndex];
             
                textBox3.Text = row.Cells[1].Value.ToString();
                if (textBox3.Text.Substring(0, 1) == "M")
                {
                    tabControlSve.SelectedTab = tabPgMonitori;
                    textboxMonitoriSearch.Text= row.Cells[1].Value.ToString();
                }
                 if (textBox3.Text.Substring(0,1)=="P")  
                {
                    tabControlSve.SelectedTab = tabPgPrinteri;
                    textboxPrinteriSearch.Text = row.Cells[1].Value.ToString();
                }
                if (textBox3.Text.Substring(0, 1) == "S") 
                {
                    tabControlSve.SelectedTab = tabPgSkeneri;
                    textboxSkeneriSearch.Text = row.Cells[1].Value.ToString();
                }
                if (textBox3.Text.Substring(0, 1) == "W")
                {
                    tabControlSve.SelectedTab = tabPgPrespojnici;
                    textboxSwitchSearch.Text = row.Cells[1].Value.ToString();
                }

            }

            //    
        }

        private void btnModeliSvegaConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                // Potvrda = '"
                // + checkBoxRac.Checked + "',
                string query = "update ModelNazivi set  Napomena='" + richTextBox1.Text + "' where Model= '" + comboBoxModeli.SelectedItem + "' ";


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                con.Close();
            
                btnModeliSvegaUpd.PerformClick();

            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error!" + ex);

            }
        }

        private void btnModeliSvegaUpd_Click(object sender, EventArgs e)
        {
            con.Open();
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select  Model, Napomena from ModelNazivi'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            modelNaziviDataGridView.DataSource = dt;
            con.Close();
        }

        private void racunalaDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.racunalaDataGridView.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxRacunala.Text = row.Cells[0].Value.ToString();
                textboxRacunalaNaz.Text = row.Cells[1].Value.ToString();
                textboxRacunalaKor.Text = row.Cells[2].Value.ToString();
                textboxRacunalaMod.Text = row.Cells[3].Value.ToString();
                textboxRacunalaSB.Text = row.Cells[4].Value.ToString();
                textboxRacunalaNap.Text = row.Cells[5].Value.ToString();
                textboxRacunalaMAC.Text = row.Cells[6].Value.ToString();
                textboxRacunalaBarLok.Text = row.Cells[7].Value.ToString();
                textboxRacunalaSearch.Text = row.Cells[7].Value.ToString();
                textboxRacunalaOznLok.Text = row.Cells[9].Value.ToString();
                textboxRacunalaProstor.Text = row.Cells[10].Value.ToString();
                textboxRacunalaKat.Text = row.Cells[11].Value.ToString();
                textboxRacunalaSoba.Text = row.Cells[12].Value.ToString();
                DataGridViewCheckBoxCell chk = row.Cells[8] as DataGridViewCheckBoxCell;



                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxRac.Checked = true;
                }
                else checkBoxRac.Checked = false;


            }
        }

        private void checkBoxHint_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHint.Checked==true)
            {
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                label14.Visible = true;
                label15.Visible = true;
                label16.Visible = true;
                label17.Visible = true;
                label18.Visible = true;
                label19.Visible = true;
              
            }
            else if (checkBoxHint.Checked == false)
                {
                    label1.Visible = false;
                    label2.Visible = false;
                    label3.Visible = false;
                    label4.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;
                    label7.Visible = false;
                    label8.Visible = false;
                    label9.Visible = false;
                    label10.Visible = false;
                    label11.Visible = false;
                    label12.Visible = false;
                    label13.Visible = false;
                    label14.Visible = false;
                    label15.Visible = false;
                    label16.Visible = false;
                    label17.Visible = false;
                    label18.Visible = false;
                    label19.Visible = false;
                }
            
        }

        private void monitoriDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.monitoriDataGridView.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxMonitoriSearch.Text = row.Cells[4].Value.ToString();
                textboxMonitoriBarcode.Text = row.Cells[0].Value.ToString();
                textboxMonitoriModel.Text = row.Cells[1].Value.ToString();
                textboxMonitoriSrBr.Text = row.Cells[2].Value.ToString();
                textboxMonitoriNapomena.Text = row.Cells[3].Value.ToString();
                textboxMonitoriBarcodeLokacije.Text = row.Cells[4].Value.ToString();
                DataGridViewCheckBoxCell chk = row.Cells[5] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxMntr.Checked = true;
                }
                else checkBoxMntr.Checked = false;

            }
        }

        private void printeriDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.printeriDataGridView.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxPrinteriBarcode.Text = row.Cells[0].Value.ToString();
                textboxPrinteriModel.Text = row.Cells[1].Value.ToString();
                textboxPrinteriSrBr.Text = row.Cells[2].Value.ToString();
                textboxPrinteriNapomena.Text = row.Cells[3].Value.ToString();
                textboxPrinteriIPadresa.Text = row.Cells[4].Value.ToString();
                textboxPrinteriBarcodeLokacije.Text = row.Cells[5].Value.ToString();
                textboxPrinteriSearch.Text = row.Cells[5].Value.ToString();
                DataGridViewCheckBoxCell chk = row.Cells[6] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxPrnt.Checked = true;
                }

            }
        }

        private void skeneriDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.skeneriDataGridView.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxSkeneriBarcode.Text = row.Cells[0].Value.ToString();
                textboxSkeneriModel.Text = row.Cells[1].Value.ToString();
                textboxSkeneriSrBr.Text = row.Cells[2].Value.ToString();
                textboxSkeneriNapomena.Text = row.Cells[3].Value.ToString();
                textboxSkeneriBarcodeLokacije.Text = row.Cells[4].Value.ToString();
                textboxSkeneriSearch.Text = row.Cells[0].Value.ToString();

                textboxSkeneriSearch.Text = row.Cells[4].Value.ToString();


                DataGridViewCheckBoxCell chk = row.Cells[5] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxScn.Checked = true;
                }
                else checkBoxScn.Checked = false;



            }
        }

        private void prespojniciDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.prespojniciDataGridView.Rows[e.RowIndex];
                //popuni textboxove gdje se upisuju values ukoliko kliknemo na grid view
                //isto se moze napraviti jednostavno draganjem sa data sourcea cijelog tablea
                textboxSwitchBarcode.Text = row.Cells[0].Value.ToString();
                textboxSwitchModel.Text = row.Cells[1].Value.ToString();
                textboxSwitchSrBr.Text = row.Cells[2].Value.ToString();
                textboxSwitchNapomena.Text = row.Cells[3].Value.ToString();
                textboxSwBarcodeLokacije.Text = row.Cells[4].Value.ToString();



                textboxSwitchSearch.Text = row.Cells[4].Value.ToString();


                DataGridViewCheckBoxCell chk = row.Cells[5] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value) == true)
                {
                    checkBoxSw.Checked = true;
                }
                else checkBoxSw.Checked = false;
            }
        }



        /*
        private const int Margin = 10;
        private void tipToolRac_Popup(object sender, PopupEventArgs e)
        {


            var imageName = string.Format("{0}.jpg", textboxRacunalaMod.ToString());
            var file = System.IO.Path.Combine(Application.StartupPath, "Resources", imageName);
            Image image1 = Image.FromFile(file);


            int image_wid = 2 * Margin + image1.Width;
            int image_hgt = 2 * Margin + image1.Height;

            int wid = e.ToolTipSize.Width + 2 * Margin + image_wid;
            int hgt = e.ToolTipSize.Height;
            if (hgt < image_hgt) hgt = image_hgt;

            e.ToolTipSize = new Size(wid, hgt);
        }

        private void tipToolRac_Draw(object sender, DrawToolTipEventArgs e)
        {

            var imageName = string.Format("{0}.jpg", textboxRacunalaMod.ToString());
            var file = System.IO.Path.Combine(Application.StartupPath, "Resources", imageName);
            Image image1 = Image.FromFile(file);
            // Draw the background and border.
            e.DrawBackground();
            e.DrawBorder();

            // Draw the image.
            e.Graphics.DrawImage(image1, Margin, Margin);

            // Draw the text.
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                int image_wid = 2 * Margin + image1.Width;

                Rectangle rect = new Rectangle(image_wid, 0,
                    e.Bounds.Width - image_wid, e.Bounds.Height);
                e.Graphics.DrawString(
                    e.ToolTipText, e.Font, Brushes.Green, rect, sf);
            }

        }
        */
    }
    

}

