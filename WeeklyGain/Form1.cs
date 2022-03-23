using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeeklyGain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region Variables
        string mWallet = "";
        bool mShowWallet = true;
        bool mShowBalance = true;
        Account acc = new Account();
        PersonalData PersonalInfo = new PersonalData();
        #endregion

        private void LoadValuesFromResources()
        {
            if (File.Exists("InternalStorage"))
            {
                ResourceReader res = new ResourceReader("InternalStorage");
                IDictionaryEnumerator dict = res.GetEnumerator();
                while (dict.MoveNext())
                {
                    if (dict.Key.ToString().Equals("Wallet"))
                    {
                        mWallet = dict.Value.ToString();
                    }
                    if (dict.Key.ToString().Equals("ShowWallet"))
                    {
                        mShowWallet = (bool)dict.Value;
                    }
                    if (dict.Key.ToString().Equals("ShowBalance"))
                    {
                        mShowBalance = (bool)dict.Value;
                    }
                }
                res.Close();
            }
            if (mWallet == "")
            {
                Form2 frm = new Form2();
                frm.ShowDialog(this);
                mWallet = frm.result;
            }
            acc = new Account(mWallet);
        }
        private void LoadPersonalInfoToScreen()
        {
            if (PersonalInfo.heroes != null && PersonalInfo.heroes.Heroes!=null && PersonalInfo.heroes.Heroes.Count > 0)
            {
                if (PersonalInfo.PersonalProfile != null)
                {
                    if (PersonalInfo.PersonalProfile.created != 0)
                    {
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(PersonalInfo.PersonalProfile.created).ToLocalTime();
                        labPersonalInfo.Text = "Your Account was created on " + dateTime.ToShortDateString() + ", your name is " + PersonalInfo.PersonalProfile.name + " and you own " + PersonalInfo.heroes.Heroes.Count.ToString() + " Heroes";
                    }
                    else
                    {
                        labPersonalInfo.Text = "Your name is " + PersonalInfo.PersonalProfile.name + " and you own " + PersonalInfo.heroes.Heroes.Count.ToString() + " Heroes";
                    }
                }
                else { labPersonalInfo.Text = ""; }
                listHeroes.Items.Clear();
                foreach (Hero item in PersonalInfo.heroes.Heroes)
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(item.staminaFullAt).ToLocalTime();
                    TimeSpan HowMuchTime = dateTime - DateTime.Now;
                    ListViewItem itm = new ListViewItem(new string[] { item.id , PersonalData.GetRarityString(item.rarity), item.mainClass,item.profession , dateTime.ToLongTimeString (), HowMuchTime.Hours.ToString () + ":" + HowMuchTime.Minutes.ToString()}, -1);
                    listHeroes.Items.Add(itm);
                    
                }
            }
        }
        private void LoadValuesToScreen()
        {
            if (acc == null) return;
            this.SuspendLayout();
            textBloater.Text = acc.Bloaters.ToString();
            textIronScale.Text = acc.IronScales.ToString();
            textLanterney.Text = acc.Lanterney.ToString();
            textRedGill.Text = acc.RedGill.ToString();
            textSailFish.Text = acc.SailFish.ToString();
            textShimerscale.Text = acc.Shimerscale.ToString();
            textSilverfin.Text = acc.Silverfin.ToString();

            int total = 0;
            textGoldBloaters.Text = (acc.Bloaters * 1).ToString(); total += acc.Bloaters * 1;
            textGoldIronScale.Text = (acc.IronScales * 5).ToString(); total += acc.IronScales * 5;
            textGoldLanterney.Text = (acc.Lanterney * 5).ToString(); total += acc.Lanterney * 5;
            textGoldRedGill.Text = (acc.RedGill * 15).ToString(); total += acc.RedGill * 15;
            textGoldSailFish.Text= (acc.SailFish * 50).ToString(); total += acc.SailFish * 50;
            textGoldShimerscale.Text = (acc.Shimerscale * 60).ToString(); total += acc.Shimerscale * 60;
            textGoldSilverfin.Text = (acc.Silverfin * 100).ToString(); total += acc.Silverfin * 100;

            textTotalGold.Text = total.ToString ();

            textTears.Text = acc.Tears.ToString();
            textBlueEgg.Text = acc.BlueEggs.ToString();
            textRunes.Text = acc.Runes.ToString();

            labWallet.Text = mWallet;
            this.ResumeLayout (true);
        }
        private void FixDates()
        {
            dateTo.Value = DateTime.Today;

            for (int i = 0; i < 8; i++)
            {
                DateTime dateForButton = DateTime.Now.AddDays(-i);
                if (dateForButton.DayOfWeek.ToString().ToLower().Equals("sunday"))
                {
                    dateFrom.Value = dateForButton;
                    break;
                }
            }
        }
       
        
        #region Account
        private void LoadPersonalInfo()
        {
            pictLoadingPersonal.Visible = true;
            PersonalInfo.LoadHeroes(mWallet);
            LoadPersonalInfoToScreen();
            pictLoadingPersonal.Visible = false;
        }
        private void LoadAccount()
        {
            picLoadingBalance.Visible = true;

            labBalance.Text = acc.LoadBalance().ToString();
            picLoadingBalance.Visible = false;
            acc.LoadTransactionCount();
        }
        private void LoadFromTransactions(DateTime from, DateTime to)
        {
            picLoadingTransactions.Visible = true;
            picLoadingItems.Visible = true;
            acc.LoadTransactionCount(from.Date, to.Date);
            if (acc.DatesHashes != null) labTransCount.Text = acc.DatesHashes.Count.ToString();
            picLoadingTransactions.Visible = false;
            acc.LoadItemsFromTransactions();
            picLoadingItems.Visible = false;
        }
        private void LoadFromTransactions(int TransactionCount)
        {
            picLoadingTransactions.Visible = true;
            picLoadingItems.Visible = true;
            acc.LoadTransactionCount(TransactionCount);
            if (acc.DatesHashes != null) labTransCount.Text = acc.DatesHashes.Count.ToString();
            picLoadingTransactions.Visible = false;
            acc.LoadItemsFromTransactions();
            picLoadingItems.Visible = false;
        }
        #endregion
        #region Form Events
        private void Form1_Load(object sender, EventArgs e)
        {
            FixDates();
            LoadValuesFromResources();
            FixWalletShowHide();
            FixBalanceShowHide();
            InitializeList();
            delay.Interval = 40;
            delay.Start();
            delay.Tick += Delay_Tick;
        }
        Timer delay = new Timer();
        private void Delay_Tick(object sender, EventArgs e)
        {

            delay.Stop();
            delay.Tick -= Delay_Tick;

            LoadAccount();
            if (acc.CompletedHashes != null) labTransCount.Text = acc.CompletedHashes.Count.ToString();
            picLoadingTransactions.Visible = false;

            LoadFromTransactions(dateFrom.Value, dateTo.Value);
            LoadValuesToScreen();

            LoadPersonalInfo();
        }

        private void labWallet_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog(this);
            mWallet = frm.result;
            acc = new Account(mWallet);

            LoadAccount();
            LoadValuesToScreen();
        }
        private void ReLoadMethod()
        {
            if (radioDates.Checked) LoadFromTransactions(dateFrom.Value, dateTo.Value);
            else if (radioLastTransaction.Checked) LoadFromTransactions(1);
            else if (radioLastTransaction2.Checked) LoadFromTransactions(2);
            else if (radioLastTransaction3.Checked) LoadFromTransactions(3);
            LoadValuesToScreen();
        }
        private void butReload_Click(object sender, EventArgs e)
        {
            ReLoadMethod();
        }
        private void butPersonalReload_Click(object sender, EventArgs e)
        {
            LoadPersonalInfo();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            IResourceWriter writer = new ResourceWriter("InternalStorage");
            writer.AddResource("Wallet", mWallet);
            writer.AddResource("ShowWallet", mShowWallet);
            writer.AddResource("ShowBalance", mShowBalance);
            writer.Close();
        }
        
        private void labWallet2_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog(this);
            mWallet = frm.result;
            acc = new Account(mWallet);

            LoadAccount();
            LoadValuesToScreen();
        }
        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDates.Checked) groupDates.Enabled = true;
            else groupDates.Enabled = false;

            if (radioLastTransaction.Checked || radioLastTransaction2.Checked || radioLastTransaction3.Checked) ReLoadMethod();
        }
        #endregion
        #region Show/Hide Balance
        private void FixWalletShowHide()
        {
            if (mShowWallet) { picShowWallet.Image = global::WeeklyGain.Properties.Resources.Show; labWallet.Visible = true; }
            else { picShowWallet.Image = global::WeeklyGain.Properties.Resources.DontShow; labWallet.Visible = false; }

        }
        private void FixBalanceShowHide()
        {
            if (mShowBalance) { picShowBalance.Image = global::WeeklyGain.Properties.Resources.Show; labBalance.Visible = true; }
            else { picShowBalance.Image = global::WeeklyGain.Properties.Resources.DontShow; labBalance.Visible = false; }
        }
        private void picShowWallet_Click(object sender, EventArgs e)
        {
            mShowWallet = !mShowWallet;
            FixWalletShowHide();
        }

        private void picShowBalance_Click(object sender, EventArgs e)
        {
            mShowBalance = !mShowBalance;
            FixBalanceShowHide();
        }
        private void InitializeList()
        {
            listHeroes.Columns.Clear();
            listHeroes.Columns.Add("id", "ID" , 55, HorizontalAlignment.Left, -1);
            listHeroes.Columns.Add("Rarity", "Rarity", 80, HorizontalAlignment.Left, -1);
            listHeroes.Columns.Add("MainClass", "Main Class", 80, HorizontalAlignment.Left, -1);
            listHeroes.Columns.Add("profession", "Profession", 80, HorizontalAlignment.Left, -1);
            listHeroes.Columns.Add("FullStamina", "Full Stamina", 100, HorizontalAlignment.Left, -1);
            listHeroes.Columns.Add("FullStaminaIn", "Full Stamina In", 100, HorizontalAlignment.Left, -1);
        }

        #endregion
    }
}
