namespace labwork;

public class Cat
{
    public string Name { get; set; }
    public int Age { get; set; }
    public int HungerLevel { get; set; }
    public int HappinessLevel { get; set; }
    public bool IsSleeping { get; set; }
    public string Avatar { get; set; } 
    public string HappinessErrorMessage { get; set; }
    public string HungerErrorMessage { get; set; }
    private const string lowCat = "https://i.gifer.com/22Cl.gif";
    private const string midCat = "https://i.gifer.com/zfF.gif";
    private const string highCat = "https://i.gifer.com/Aq.gif";
    public Cat(string name)
    {
        Name = name;
        Age = new Random().Next(1, 10);
        HungerLevel = 50; 
        HappinessLevel = 50; 
        Avatar = "https://i.gifer.com/TIAQ.gif";
    }

    public void Feed()
    {
        if (IsSleeping)
            Console.WriteLine("Спящего кота нельзя кормить");
        else
        {
            HungerLevel += 15;
            HappinessLevel += 5;
            if (HungerLevel > 100)
            {
                HappinessLevel -= 30;
            }
            ChangeAvatar();
            CheckCharacteristics();
        }
    }

    public void Play()
    {
        if (IsSleeping)
        {
            HappinessLevel -= 5;
            HungerLevel -= 10;
            IsSleeping = false;
        }
        else
        {
            HappinessLevel += 15;
            HungerLevel -= 10;
            if (new Random().Next(1, 4) == 1)
            {
                HappinessLevel = 0;
            }
        }
        ChangeAvatar();
        CheckCharacteristics();
    }

    public void Sleep()
    {
        IsSleeping = true;
        ChangeAvatar();
    }

    public void Heal()
    {
        HappinessLevel += 20;
        HungerLevel += 15;
        Avatar = "https://i.gifer.com/8au.gif";
        CheckCharacteristics();
    }

    private void ChangeAvatar()
    {
        if (HappinessLevel < 30)
        {
            Avatar = lowCat;
        }
        else if (HappinessLevel < 70)
        {
            Avatar = midCat;
        }
        else
        {
            Avatar = highCat;
        }
    }

    public void CheckCharacteristics()
    {
        if (HappinessLevel > 100 )
        {
            HappinessErrorMessage = "Кот очень счастлив, но его показатель счастья не может быть больше 100";
            HappinessLevel = 100;
        }
        else if (HappinessLevel < 0)
        {
            HappinessErrorMessage = "Кот скоро будет в дефрессии, поиграйте с ним";
            HappinessLevel = 0;
        }
        else
        {
            HappinessErrorMessage = "";
        }
        if(HungerLevel > 100)
        {
            HungerErrorMessage = "Кот скоро лопнет от еды!";
            HungerLevel = 100;
        }
        else if(HungerLevel < 0)
        {
            HungerErrorMessage = "Коту срочно нужна еда";
            HungerLevel = 0;
        }
        else
        {
            HungerErrorMessage = "";
        }
    }
}