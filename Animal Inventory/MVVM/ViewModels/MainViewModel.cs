using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using NUnit.Framework;
using System.Windows;

namespace Animal_Inventory.MVVM.ViewModels
{
    class Animal
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string ImagePath { get; set; }
    }

    class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            
            AnimalType = new ObservableCollection<string>();
            Inventory = new ObservableCollection<string>();
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

        private ObservableCollection<string> _inventory;
        public ObservableCollection<string> Inventory
        {
            get { return _inventory; }
            set
            {
                _inventory = value;
                OnPropertyChanged();
            }
        }

        private string _selectedAnimal;
        public string SelectedAnimal
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

        private string _selectedType;
        public string SelectedType
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

        public bool isEdit = false;

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
            VisibilityMain = "Hidden";
            VisibilityTab = "Visible";
        }
        protected virtual void OnSave(object obj)
        {
            VisibilityMain = "Visible";
            VisibilityTab = "Hidden";
            if (isEdit)
            {
                Inventory.Insert(AnimalIndex, Name + "-" + SelectedType + "-" + SelectedDate.ToShortDateString() + "-" + ImagePath);
                isEdit = false;
            }
            else
            {
                animals.Add(new Animal { Name = Name, Type = SelectedType, Date = SelectedDate, ImagePath = ImagePath });
                foreach (var item in animals)
                {
                    Inventory.Add(item.Name + "-" + item.Type + "-" + item.Date.ToShortDateString() + "-" + item.ImagePath);
                }
                SaveToXml(Name, SelectedType, SelectedDate, ImagePath);
            }
            animals.Clear();
        }
        
        protected virtual void OnEdit(object obj)
        {
            VisibilityMain = "Hidden";
            VisibilityTab = "Visible";


            string[] content = SelectedAnimal.Split('-');
            int index = Inventory.IndexOf(SelectedAnimal);
            Inventory.RemoveAt(index);
            AnimalIndex = AnimalIndex + 1;


            Name = content[0];
            SelectedAnimal = content[1];
            SelectedDate = Convert.ToDateTime(content[2]);
            ImagePath = content[3];

            isEdit = true;
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
                ImagePath = op.FileName; 
            }
        }
        string filepath = @"C:\Users\Corne\source\repos\Animal Inventory\Animal Inventory\Save.xml";
        protected virtual void OnDelete(object obj)
        {
            if (SelectedAnimal != null)
            {
                string[] content = SelectedAnimal.Split('-');
                int index = Inventory.IndexOf(SelectedAnimal);
                Inventory.RemoveAt(index);

                var doc = XDocument.Load(filepath);
                var target = doc.Root.Descendants("ANIMAL").FirstOrDefault(x => x.Attribute("name").Value == content[0]);
                target.Remove();
                
                doc.Save(filepath);
            }           
        }
        
        private void SaveToXml(string name, string type, DateTime date, string path)
        {
            
            XDocument doc = XDocument.Load(filepath);
            XElement root = new XElement("ANIMAL");
            root.Add(new XAttribute("name", name));
            root.Add(new XAttribute("type", type));
            root.Add(new XAttribute("date", date.ToShortDateString()));
            root.Add(new XAttribute("path", path));
            doc.Element("ANIMALS").Add(root);
            doc.Save(filepath);
        }

        private void LoadToList()
        {
            var animal = new Animal();            
            List<Animal> lines = new List<Animal>();

            XDocument doc = XDocument.Load(filepath);
            XAttribute attribute = null;

            foreach (var item in doc.Root.Elements())
            {
                attribute = item.Attribute("path");
                if(attribute != null)
                    animal.ImagePath = attribute.Value;

                attribute = item.Attribute("name");
                if (attribute != null)
                    animal.Name = attribute.Value;

                attribute = item.Attribute("date");
                if (attribute != null)
                    animal.Date = DateTime.Parse(attribute.Value);

                attribute = item.Attribute("type");
                if (attribute != null)
                    animal.Type = attribute.Value;

                lines.Add(animal);

                foreach (Animal text in lines)
                {
                    ImagePath = text.ImagePath;
                    Inventory.Add(text.Name + "-" + text.Type + "-" + text.Date.ToShortDateString() + "-" + text.ImagePath);
                }

                lines.Clear();
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
