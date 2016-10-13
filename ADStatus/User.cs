using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class User
{
    private string firstName = "";
    private string lastName = "";
    private string Lan = "";
    private string empNum = "";
    private string status = "";
    private bool locked = false;
    private string store = "";
    private string dept = "";
    private string phone = "";
    private string expired = "";
    private string expDate = "";
    private string daysLeft = "";
    private string container = "";

    public string getContainer()
    {
        return this.container;
    }

        public void setContainer(string container)
        {
            this.container = container;
        }

    public string getDaysLeft()
    {
        return this.daysLeft;
    }

        public void setDaysLeft(string daysLeft)
        {
            this.daysLeft = daysLeft;
        }

    public string getExpDate()
    {
        return this.expDate;
    }

        public void setExpDate(string expDate)
        {
            this.expDate = expDate;
        }

    public string getExpired()
    {
        return this.expired;
    }

        public void setExpired(string expired)
        {
            this.expired = expired;
        }

    public string getPhone()
    {
        return this.phone;
    }

        public void setPhone(string phone)
        {
            this.phone = phone;
        }

    public string getDept()
    {
        return this.dept;
    }

        public void setDept(string dept)
        {
            this.dept = dept;
        }

    public string getStore()
    {
        return this.store;
    }

        public void setStore(string store)
        {
            this.store = store;
        }

    public bool getLocked()
    {
        return this.locked;
    }

        public void setLocked(bool locked)
        {
            this.locked = locked;
        }

    public string getFirstName()
    {
        return firstName;
    }

        public void setFirstName(string name)
        {
            this.firstName = name;
        }

    public string getLastName()
    {
        return lastName;
    }

        public void setLastName(string name)
        {
            this.lastName = name;
        }

    public string getLan()
    {
        return Lan;
    }

        public void setLan(string Lan)
        {
            this.Lan = Lan;
        }

    public string getEmpNum()
    {
        return empNum;
    }

        public void setEmpNum(string empNum)
        {
            this.empNum = empNum;
        }

    public string getStatus()
    {

        return this.status;
    }

        public void setStatus(string stat)
        {
            this.status = stat;
        }

    public void clearUser()
    {
        this.status = "";
        this.empNum = "";
        this.Lan = "";
        this.firstName = "";
        this.lastName = "";
        this.store = "";
        this.locked = false;
        this.status = "";
        this.container = "";
        this.daysLeft = "";
        this.dept = "";
        this.expDate = "";
        this.expired = "";
        this.phone = "";
        this.store = "";
    }
}
