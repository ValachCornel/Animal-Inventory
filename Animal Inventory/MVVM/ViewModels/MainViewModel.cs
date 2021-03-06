using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Linq;
using System.Windows;
using System.IO;

namespace Animal_Inventory.MVVM.ViewModels
{
    public class Animal
    {
        public string Name { get; set; }
        public AnimalType Type { get; set; }
        public DateTime Date { get; set; }
        public string ImagePath { get; set; }
    }

    public enum AnimalType
    {
        [Description("Niciunu")]
        [System.ComponentModel.DataAnnotations.Display(Order = 0)]
        None = -1,
        [Description("Animal mic")]
        [System.ComponentModel.DataAnnotations.Display(Order = 1)]
        AnimalMic = 0,
        [Description("Animal mare")]
        [System.ComponentModel.DataAnnotations.Display(Order = 2)]
        AnimalMare = 1

    }

    class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            
            AnimalType = new ObservableCollection<string>();
            _inventory = new ObservableCollection<Animal>();
            AnimalType.Add("Animal Mare");
            AnimalType.Add("Animal Mic");
            SelectedDate = DateTime.Now;
            VisibilityTab = "Hidden";
            InitializeCommands();
            LoadToList();
            ImagePath = @"C:\Users\Corne\source\repos\Animal Inventory\Animal Inventory\Photos\download.jpg";
        }

        #region Binding

        List<Animal> animals = new List<Animal>();

       

        private string _visibilityMain;
        public string VisibilityMain
        {
            get { return _visibilityMain; }
            set
            {
                _visibilityMain = value;
                OnPropertyChanged();
            }
        }

        private string _visibilityTab;
        public string VisibilityTab
        {
            get { return _visibilityTab; }
            set
            {
                _visibilityTab = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Animal> _inventory;
        public ObservableCollection<Animal> Inventory
        {
            get { return _inventory; }
            set
            {
                _inventory = value;
                OnPropertyChanged();
            }
        }

        private Animal _selectedAnimal;
        public Animal SelectedAnimal
        {
            get { return _selectedAnimal; }
            set
            {
                _selectedAnimal = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _animalType;
        public ObservableCollection<string> AnimalType
        {
            get { return _animalType; }
            set
            {
                _animalType = value;
                OnPropertyChanged();
            }
        }

        private AnimalType _selectedType;
        public AnimalType SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private int _animalIndex;
        public int AnimalIndex
        {
            get { return _animalIndex; }
            set
            {
                _animalIndex = value;
                OnPropertyChanged();
            }
        }

        public bool isAdd = false;
        public bool isEdit = false;
        string filepath = Directory.GetCurrentDirectory() + "\\Save.xml";
        string folderPath = Directory.GetCurrentDirectory() + "\\Photos\\";

        #endregion

        public DelegateCommand<object> AddCommand { get; private set; }
        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> EditCommand { get; private set; }
        public DelegateCommand<object> SelectCommand { get; private set; }
        public DelegateCommand<object> DeleteCommand { get; private set; }
        protected virtual void InitializeCommands()
        {

            AddCommand = new DelegateCommand<object>(OnAdd);
            SaveCommand = new DelegateCommand<object>(OnSave);
            EditCommand = new DelegateCommand<object>(OnEdit);
            SelectCommand = new DelegateCommand<object>(OnSelect);
            DeleteCommand = new DelegateCommand<object>(OnDelete);
        }

        protected virtual void OnAdd(object obj)
        {
            isAdd = true;
            Name = "";
            ImagePath = "";
            SelectedType = ViewModels.AnimalType.None;
            SelectedDate = DateTime.Now;
            VisibilityMain = "Hidden";
            VisibilityTab = "Visible";
        }
        protected virtual void OnSave(object obj)
        {
            if (isAdd)
            {
                VisibilityMain = "Visible";
                VisibilityTab = "Hidden";
                if (Name == "" || SelectedType == ViewModels.AnimalType.None || SelectedDate == null || ImagePath == null)
                {
                    MessageBox.Show("Please fill all the fields!");
                }
                else
                {
                    if (isEdit)
                    {
                        Inventory.Insert(AnimalIndex, new Animal { Name = Name, Type = SelectedType, Date = SelectedDate.Date, ImagePath = Path.Combine(folderPath, ImagePath) });
                        SaveOnEdit(selectedName);
                        isEdit = false;
                    }
                    else
                    {
                        animals.Add(new Animal { Name = Name, Type = SelectedType, Date = SelectedDate.Date, ImagePath = Path.Combine(folderPath, ImagePath) });
                        foreach (var item in animals)
                        {
                            Inventory.Add(new Animal { Name = item.Name, Type = item.Type, Date = item.Date.Date, ImagePath = Path.GetFileName(item.ImagePath) });
                        }
                        SaveToXml(Name, SelectedType.ToString(), SelectedDate, ImagePath);
                    }

                    Inventory.Clear();
                    LoadToList();
                    animals.Clear();

                }
                isAdd = false;
            }
                      
            
        }
        string selectedName, path;
        protected virtual void OnEdit(object obj)
        {
            if(SelectedAnimal != null)
            {
                VisibilityMain = "Hidden";
                VisibilityTab = "Visible";

                selectedName = SelectedAnimal.Name;
                path = SelectedAnimal.ImagePath;
                AnimalType type = SelectedAnimal.Type;
                string date = SelectedAnimal.Date.ToString();

                int index = Inventory.IndexOf(SelectedAnimal);
                Inventory.RemoveAt(index);
                AnimalIndex = AnimalIndex + 1;

                Name = selectedName;
                SelectedType = type;
                SelectedDate = Convert.ToDateTime(date);
                ImagePath = path;

                isEdit = true;
            }
            else
                MessageBox.Show("Please select a animal!");     
        }
        protected virtual void OnSelect(object obj)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (!File.Exists(Path.Combine(folderPath, op.SafeFileName)))
                {
                    ImagePath = op.SafeFileName;
                    File.Copy(op.FileName, Path.Combine(folderPath, Path.GetFileName(ImagePath)), true);
                }
                else
                    ImagePath = op.SafeFileName;
                                             
            }
        }       
        protected virtual void OnDelete(object obj)
        {
            if (SelectedAnimal != null)
            {         
                var doc = XDocument.Load(filepath);
                var target = doc.Root.Descendants("ANIMAL").FirstOrDefault(x => x.Attribute("name").Value == SelectedAnimal.Name.ToString());
                target.Remove();
                int index = Inventory.IndexOf(SelectedAnimal);
                Inventory.RemoveAt(index);                
                doc.Save(filepath);
            }
            else
                MessageBox.Show("Please select a animal!");
                       
        }
        
        private void SaveToXml(string name, string type, DateTime date, string path)
        {
            XDocument doc = null;

            if (File.Exists(filepath))
                doc = XDocument.Load(filepath);
            else
                doc = new XDocument(new XElement("ANIMALS"));

            XElement root = new XElement("ANIMAL");
            root.Add(new XAttribute("name", name));
            root.Add(new XAttribute("type", type));
            root.Add(new XAttribute("date", date.ToShortDateString()));
            root.Add(new XAttribute("path", path));
            doc.Element("ANIMALS").Add(root);
            doc.Save(filepath);
        }

        private void SaveOnEdit(string name)
        {
            var doc = XDocument.Load(filepath);
            var target = doc.Root.Descendants("ANIMAL").FirstOrDefault(x => x.Attribute("name").Value == name);
            target.Attribute("path").Value = ImagePath;
            target.Attribute("name").Value = Name;
            target.Attribute("date").Value = SelectedDate.ToShortDateString();
            target.Attribute("type").Value = SelectedType.ToString();
            doc.Save(filepath);
        }

        private void LoadToList()
        {                      
            List<Animal> lines = new List<Animal>();
            XDocument doc = null;

            if (File.Exists(filepath))
             doc = XDocument.Load(filepath);

            if (doc == null) return;

            XAttribute attribute = null;

            foreach (var item in doc.Root.Elements())
            {
                var animal = new Animal();

                attribute = item.Attribute("path");
                if(attribute != null)
                    animal.ImagePath = Path.Combine(folderPath, attribute.Value);

                attribute = item.Attribute("name");
                if (attribute != null)
                    animal.Name = attribute.Value;

                attribute = item.Attribute("date");
                if (attribute != null)
                    animal.Date = DateTime.Parse(attribute.Value).Date;

                attribute = item.Attribute("type");
                if (attribute != null)
                {
                    Enum.TryParse(attribute.Value, out AnimalType animalType);
                    animal.Type = animalType;
                }


                Inventory.Add(animal);

            }                               
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
