using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;
using System.Data.Entity;

namespace project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        int _ColumnsTab = 2;
        int indexElem;
        double scale;
        int indexProject = -1;


        public MainWindow()
        {
            InitializeComponent();
            //неактивная кнопка развертывания
            ButtonWindowResrore.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            //формирование рабочего поля
            UpdateBackPattern(null, null);
            //заполнение элементов при старте программы
            Fill(PlantComboBox);
            Fill(StructureListView);
            Fill(ListProject);
            //создание проекта
            WorkingField.Opacity = 0.0;
            WorkingField.Visibility = Visibility.Hidden;
            WorkingField.IsEnabled = false;
            WindowNewProject.Opacity = 1.0;
            //установление значений по умолчанию
            SetSizeStructureBox.Opacity = 0.0;
            SetSizeStructureBox.Visibility = Visibility.Hidden;
            SetSizeStructureBox.IsEnabled = false;
            ContainerStructure.Opacity = 1.0;
            //создание проекта
            OlpProjectWindow.Opacity = 0.0;
            OlpProjectWindow.Visibility = Visibility.Hidden;
            OlpProjectWindow.IsEnabled = false;
            NewProjectWindow.Opacity = 1.0;
        }

        //заполнение элементов при старте программы
        private void Fill(object elem)
        {
            using (LandscapeDesignEntities db = new LandscapeDesignEntities())
            {
                //заполнение ComboBox - виды растений
                if (elem is ComboBox)
                {
                    //определение элемента
                    if ((elem as ComboBox).Name.ToString() == "PlantComboBox")
                    {
                        //добавление значений в элемент
                        TimeBox tb = new TimeBox() { Name = "все" };
                        PlantComboBox.Items.Add(tb);
                        //добавление значений в элемент из базы
                        foreach (var item in db.PlantSpecies.ToList())
                        {
                            tb = new TimeBox() { Name = item.PlantSpecies1 };
                            PlantComboBox.Items.Add(tb);
                        }
                        //поле для вывода
                        PlantComboBox.DisplayMemberPath = "Name";
                    }
                    //активный элемент
                    PlantComboBox.SelectedIndex = 0;
                }
                else if (elem is ListView)
                {
                    if ((elem as ListView).Name.ToString() == "StructureListView")
                    {
                        //очистка содержимого блока
                        StructureListView.Items.Clear();
                        foreach (var item in db.Structures)
                        {
                            string path = Directory.GetCurrentDirectory();
                            //вывод всех строений без фильтка
                            StructureListView.Items.Add(new ExplorerData()
                            {
                                //формирование вывода строений
                                ID = item.IDStructure,
                                Name = item.StructureName,
                                Icon = new BitmapImage(new Uri(path + "\\..\\..\\picture\\" + item.StructurePicture, UriKind.Relative))
                            });
                        }
                    }
                }
                else if (elem is ListBox)
                {
                    if ((elem as ListBox).Name.ToString() == "ListProject")
                    {
                        ListProject.Items.Clear();
                        foreach (var item in db.Projects)
                        {
                            ListProject.Items.Add(new ExplorerData()
                            {
                                ID = item.IDProject,
                                Name = item.Project
                            });
                        }
                    }
                }
            }
        }

        //формирование рабочего поля
        private void UpdateBackPattern(object sender, SizeChangedEventArgs e)
        {
            //определение размера рабочей области
            var w = BackgroundCanvas.ActualWidth;
            var h = BackgroundCanvas.ActualHeight;
            BackgroundCanvas.Children.Clear();
            //добавление сетки по горизонтали
            for (int x = 10; x < w; x += 10)
                AddLineToBackground(x, 0, x, h);
            //добавление сетки по вертикали
            for (int y = 10; y < h; y += 10)
                AddLineToBackground(0, y, w, y);
        }

        //прорисовка линии для сетки на главном рабочем поле
        private void AddLineToBackground(double x1, double y1, double x2, double y2)
        {
            //создание линии
            var line = new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            };
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            //добавление линии на рабочее поле
            BackgroundCanvas.Children.Add(line);
        }

        //фильтрация вывода растений
        private void PrintPicture(object sender, SelectionChangedEventArgs e)
        {
            //проверка на наничие выбраного элемента
            if (PlantComboBox.SelectedIndex != -1)
            {
                using (LandscapeDesignEntities db = new LandscapeDesignEntities())
                {
                    //определение текущего выбраного элемента
                    int index = PlantComboBox.SelectedIndex;
                    //очистка содержимого блока
                    PlantListView.Items.Clear();
                    foreach (var item in db.Plants)
                    {
                        string path = Directory.GetCurrentDirectory();
                        //вывод всех растений без фильтка
                        if (index == 0)
                        {
                            PlantListView.Items.Add(new ExplorerData()
                            {
                                //формирование вывода растений
                                ID = item.IDPlant,
                                Name = item.PlantName,
                                Icon = new BitmapImage(new Uri(path + "\\..\\..\\picture\\" + item.PicturePlant, UriKind.Relative))
                            });
                        }
                        //вывод растения выбраного вида
                        else
                        {
                            if (index == item.IDPlantSpecies)
                            {
                                //формирование вывода растений
                                PlantListView.Items.Add(new ExplorerData()
                                {
                                    ID = item.IDPlant,
                                    Name = item.PlantName,
                                    Icon = new BitmapImage(new Uri(path + "\\..\\..\\picture\\" + item.PicturePlant, UriKind.Relative))
                                });
                            }
                        }
                    }
                }
            }
        }

        //изменение количества колонок в Tab
        public int ColumnsTab
        {
            get { return _ColumnsTab; }
            set
            {
                _ColumnsTab = value;
                OnPropertyChanged("ColumnsTab");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
        private void OnPropertyChanged([CallerMemberName]string name = "") { PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ColumnsTab")); }

        //кнопоки шапки
        private void ButtonWindow_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Name.ToString();
            //закрытие окна
            if (name == "ButtonWindowClose")
                Application.Current.Shutdown();
            //сварачивание окна
            else if (name == "ButtonWindowMinimaze")
                WindowState = WindowState.Minimized;
            //разворачивание окна
            else if ((name == "ButtonWindowMaximize"))
            {
                WindowState = WindowState.Maximized;
                ButtonWindowMaximize.Visibility = Visibility.Collapsed;
                ButtonWindowResrore.Visibility = Visibility.Visible;
                ColumnsTab = 3;
            }
            //возвращение окна в нормальное состояние
            else if ((name == "ButtonWindowResrore"))
            {
                WindowState = WindowState.Normal;
                ButtonWindowMaximize.Visibility = Visibility.Visible;
                ButtonWindowResrore.Visibility = Visibility.Collapsed;
                ColumnsTab = 2;
            }
            //открытие меню окна
            else if ((name == "ButtonOpenMenu"))
            {
                ButtonOpenMenu.Visibility = Visibility.Collapsed;
                ButtonCloseMenu.Visibility = Visibility.Visible;
            }
            //закрытие меню окна
            else if ((name == "ButtonCloseMenu"))
            {
                ButtonCloseMenu.Visibility = Visibility.Collapsed;
                ButtonOpenMenu.Visibility = Visibility.Visible;
            }
            //сохранение проекта
            else if ((name == "ButtonWindowSave"))
            {
                SaveProject();
            }
        }

        //перемешение экрана
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //выбор экземпляра в каталоге
        private void SelectionChangedTab(object sender, SelectionChangedEventArgs e)
        {
            indexElem = 0;
            if ((sender as ListView).SelectedIndex == -1) return;
            else
            {
                //определение индекса элемента который сейчас выбран
                dynamic selectedItem = (sender as ListView).SelectedItem;
                indexElem = selectedItem.ID;
            }
            if ((sender as ListView).Name.ToString() == "StructureListView")
            {
                LengthStructureTextBox.Text = "";
                WidthStructureTextBox.Text = "";
                ShowRegistrationGrid(SetSizeStructureBox);
                ShowRegistrationGrid(ContainerStructure);
            }
        }

        //определение координаты курсора и выбор элемента
        private void CursorCoordinates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //определение координаты курсора при клике на рабочем поле
            Point point = Mouse.GetPosition(WorkingFieldCanvas);
            //нажата левая кнопка мыши
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                using (LandscapeDesignEntities db = new LandscapeDesignEntities())
                {
                    //определение на каком элементе было произведено нажатие
                    IInputElement clickedElement = Mouse.DirectlyOver;
                    if (clickedElement is Rectangle)
                    {
                        //двойное нажатие левой кнопкой на изображение
                        if (e.ClickCount == 2)
                        {
                            Rectangle img = clickedElement as Rectangle;
                            //удаление изображения
                            WorkingFieldCanvas.Children.Remove(img);
                        }
                    }
                    else
                    {
                        //выбор элементов в вкладке растения
                        if ((tabControl.SelectedItem as TabItem).Header.ToString() == "Растения")
                        {
                            //поиск элемента выбраного в каталоге
                            foreach (var item in db.Plants)
                            {
                                if (item.IDPlant == indexElem)
                                {
                                    string path = Directory.GetCurrentDirectory();
                                    //создание image
                                    Rectangle rec = new Rectangle
                                    {
                                        Width = Convert.ToInt32(item.WidthPlant) * scale / 10,
                                        Height = Convert.ToInt32(item.HeightPlant) * scale / 10,
                                        Name = item.PlantName,
                                        StrokeThickness = 0,
                                        Fill = new ImageBrush
                                        {
                                            ImageSource = new BitmapImage(new Uri(path + "\\..\\..\\picture\\" + item.PicturePlantTop))
                                        }
                                    };
                                    //отображение изображения на поле
                                    PrintPicture(rec, point);
                                }
                            }
                        }
                        //выбор элементов в вкладке строения
                        else if ((tabControl.SelectedItem as TabItem).Header.ToString() == "Строения")
                        {
                            //поиск элемента выбраного в каталоге
                            foreach (var item in db.Structures)
                            {
                                if (item.IDStructure == indexElem)
                                {
                                    string path = Directory.GetCurrentDirectory();
                                    //создание image
                                    if (WidthStructureTextBox.Text == "" || LengthStructureTextBox.Text == "")
                                    {
                                        ErrorText.Text = "Введите размер";
                                        ErrorText.Height = 30;
                                        ErrorText.FontSize = 15;
                                    }
                                    else
                                    {
                                        Rectangle rec = new Rectangle
                                        {
                                            Width = Convert.ToDouble(WidthStructureTextBox.Text) * scale,
                                            Height = Convert.ToDouble(LengthStructureTextBox.Text) * scale,
                                            Name = item.StructureName,
                                            StrokeThickness = 1,
                                            Fill = new ImageBrush
                                            {
                                                ImageSource = new BitmapImage(new Uri(path + "\\..\\..\\picture\\" + item.StructureTop))
                                            }
                                        };
                                        PrintPicture(rec, point);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //нажата правая кнопка мышы
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                //удаление активности выбраного элемента
                indexElem = 0;
                PlantListView.SelectedItem = null;
                StructureListView.SelectedItem = null;
            }
        }

        //прорисовка элемента на рабочем поле
        private void PrintPicture(Rectangle PictureImage, Point point)
        {
            //добавление image на рабочее поле
            WorkingFieldCanvas.Children.Add(PictureImage);
            //отображение элемена по горизонтали
            if (point.X - PictureImage.Width / 2 < 0)
                Canvas.SetLeft(PictureImage, 0);
            else if (point.X + PictureImage.Width / 2 > WorkingFieldCanvas.Width)
                Canvas.SetLeft(PictureImage, WorkingFieldCanvas.Width - PictureImage.Width);
            else
                Canvas.SetLeft(PictureImage, point.X - PictureImage.Width / 2);
            //отображение элемена по вертикали
            if (point.Y - PictureImage.Height / 2 < 0)
                Canvas.SetTop(PictureImage, 0);
            else if (point.Y + PictureImage.Height / 2 > WorkingFieldCanvas.Height)
                Canvas.SetTop(PictureImage, WorkingFieldCanvas.Height - PictureImage.Height);
            else
                Canvas.SetTop(PictureImage, point.Y - PictureImage.Height / 2);
        }

        //вывод информации о растении в строке состояния
        private void CursorMove(object sender, MouseEventArgs e)
        {
            using (LandscapeDesignEntities db = new LandscapeDesignEntities())
            {
                //определение элемента на который наведена мыша
                IInputElement clickedElement = Mouse.DirectlyOver;
                if (clickedElement is Rectangle)
                {
                    //поиск нужного элемента в базе
                    foreach (var item in db.Plants)
                        if ((clickedElement as Rectangle).Name.ToString() == item.PlantName)
                            //формирование текста на вывод
                            TextInfoStatusBar.Text = "Примечание: " + item.PlantInfo;
                }
                //очистка строки, если мышка наведена не на растение
                else
                    TextInfoStatusBar.Text = "";
            }
        }

        //проверка на коректность ввода значений
        private void StructureTextBox(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        //установление размеров строения
        private void SetSizeStructureButton_Click(object sender, RoutedEventArgs e)
        {
            WidthText.Foreground = Brushes.Black;
            LengthText.Foreground = Brushes.Black;
            //проверка на коректность ввода
            if (WidthStructureTextBox.Text == "" || WidthStructureTextBox.Text == "0")
                WidthText.Foreground = Brushes.Red;
            else if (LengthStructureTextBox.Text == "" || LengthStructureTextBox.Text == "0")
                LengthText.Foreground = Brushes.Red;
            //появление сообщения об ошибке 
            else if (Convert.ToDouble(WidthStructureTextBox.Text) * 15 > WorkingFieldCanvas.Width || Convert.ToDouble(LengthStructureTextBox.Text) * 15 > WorkingFieldCanvas.Height)
            {
                ErrorText.Text = "Не коректный ввод размера";
                ErrorText.Height = 30;
                ErrorText.FontSize = 15;
            }
            //установление значений
            else
            {
                ShowRegistrationGrid(SetSizeStructureBox);
                ShowRegistrationGrid(ContainerStructure);
            }
            ErrorText.Height = 0;
            ErrorText.Text = "";
        }

        //анимирование блоков складки "строения"
        private void ShowRegistrationGrid(DockPanel Grid)
        {
            DoubleAnimation stackAnimation = new DoubleAnimation();
            //анимация исчезновение окна
            if (Grid.Opacity == 1.0)
            {
                Grid.IsEnabled = false;
                Grid.Visibility = Visibility.Hidden;
                stackAnimation.From = 1.0;
                stackAnimation.To = 0.0;
                stackAnimation.Duration = TimeSpan.FromMilliseconds(800.0);
                Panel.SetZIndex(Grid, -1);
            }
            //анимация появление окна
            else if (Grid.Opacity == 0.0)
            {
                Grid.IsEnabled = true;
                Grid.Visibility = Visibility.Visible;
                stackAnimation.From = 0.0;
                stackAnimation.To = 1.0;
                stackAnimation.Duration = TimeSpan.FromMilliseconds(800.0);
                Panel.SetZIndex(Grid, 1);
            }
            Grid.BeginAnimation(DockPanel.OpacityProperty, stackAnimation);
        }

        //закрытие окна установки размера строений
        private void ButtonCloseWindowSize_Click(object sender, RoutedEventArgs e)
        {
            WidthStructureTextBox.Text = "";
            LengthStructureTextBox.Text = "";
            indexElem = 0;
            ErrorText.Text = "";
            ErrorText.Height = 0;
            StructureListView.SelectedItem = null;
            ShowRegistrationGrid(SetSizeStructureBox);
            ShowRegistrationGrid(ContainerStructure);
        }

        //повторное открытие окна установки размера строений
        private void PreviewStructureListView_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                ShowRegistrationGrid(SetSizeStructureBox);
                ShowRegistrationGrid(ContainerStructure);
            }
        }

        //установка розмеров рабочей области
        private void ButtonNewProject_Click(object sender, RoutedEventArgs e)
        {
            labelBoxNameProject.Foreground = Brushes.Black;
            labelBoxWidthProject.Foreground = Brushes.Black;
            labelBoxHeightProject.Foreground = Brushes.Black;
            //проверка на коректность ввода
            if (TextBoxNameProject.Text == "")
                labelBoxNameProject.Foreground = Brushes.Red;
            else if (TextBoxWidthProject.Text == "" || TextBoxWidthProject.Text == "0")
                labelBoxWidthProject.Foreground = Brushes.Red;
            else if (TextBoxHeightProject.Text == "" || TextBoxHeightProject.Text == "0")
                labelBoxHeightProject.Foreground = Brushes.Red;
            //появление сообщения об ошибке 
            else if (Convert.ToDouble(TextBoxWidthProject.Text) < 10 || Convert.ToDouble(TextBoxHeightProject.Text) < 10)
            {
                TextBoxErrorProject.Text = "Не коректный ввод размера";
                TextBoxErrorProject.Height = 30;
                TextBoxErrorProject.FontSize = 15;
            }
            //установление значений
            else
            {
                SetWorkingField();
                ShowRegistrationGrid(WindowNewProject);
                ShowRegistrationGrid(WorkingField);
            }
        }

        //установление масштаба рабочего поля
        private void SetWorkingField()
        {
            //задание коэфициента масштабирования
            if (Convert.ToDouble(TextBoxWidthProject.Text) <= 15 && Convert.ToDouble(TextBoxHeightProject.Text) <= 15)
                scale = 35;
            if (Convert.ToDouble(TextBoxWidthProject.Text) <= 20 && Convert.ToDouble(TextBoxHeightProject.Text) <= 20)
                scale = 20;
            else if (Convert.ToDouble(TextBoxWidthProject.Text) <= 30 && Convert.ToDouble(TextBoxHeightProject.Text) <= 30)
                scale = 15;
            else if (Convert.ToDouble(TextBoxWidthProject.Text) <= 50 && Convert.ToDouble(TextBoxHeightProject.Text) <= 50)
                scale = 11;
            else if (Convert.ToDouble(TextBoxWidthProject.Text) <= 100 && Convert.ToDouble(TextBoxHeightProject.Text) <= 100)
                scale = 5;
            else if (Convert.ToDouble(TextBoxWidthProject.Text) <= 150 && Convert.ToDouble(TextBoxHeightProject.Text) <= 150)
                scale = 3;
            else if (Convert.ToDouble(TextBoxWidthProject.Text) <= 250 && Convert.ToDouble(TextBoxHeightProject.Text) <= 250)
                scale = 2;
            else if (Convert.ToDouble(TextBoxWidthProject.Text) <= 500 && Convert.ToDouble(TextBoxHeightProject.Text) <= 500)
                scale = 1;
            else if (Convert.ToDouble(TextBoxWidthProject.Text) <= 1000 && Convert.ToDouble(TextBoxHeightProject.Text) <= 1000)
                scale = 0.5;
            else
                scale = 0.1;
            //установление размеров рабочего поля
            WorkingFieldCanvas.Width = Convert.ToDouble(TextBoxWidthProject.Text) * scale;
            WorkingFieldCanvas.Height = Convert.ToDouble(TextBoxHeightProject.Text) * scale;
            InfoStatusBar.Width = WorkingFieldCanvas.Width;
        }

        //сохранение проекта
        private void SaveProject()
        {
            using (LandscapeDesignEntities db = new LandscapeDesignEntities())
            {
                //если проект редакрируется
                if (indexProject != -1)
                {
                    //поиск растений на старом проекте
                    var queryP = from p in db.Projects
                                 from pl in db.ProjectPlants
                                 where p.IDProject == indexProject && pl.IDProject == p.IDProject
                                 select pl;
                    //удаление растений из старого проекта
                    foreach (var item in queryP)
                        db.ProjectPlants.Remove(item);
                    //поиск строений на старом проекте
                    var queryS = from p in db.Projects
                                from st in db.ProjectStructure
                                where p.IDProject == indexProject && st.IDProject == p.IDProject
                                select st;
                    //удатение строений из страрого проекта
                    foreach (var item in queryS)
                        db.ProjectStructure.Remove(item);
                    db.SaveChanges();
                }
                //если проекта еще небыло
                else
                {
                    //создание экземпляра проекта
                    Projects project = new Projects()
                    {
                        Project = TextBoxNameProject.Text,
                        Width = Convert.ToDouble(TextBoxWidthProject.Text),
                        Height = Convert.ToDouble(TextBoxHeightProject.Text),
                        Scale = scale
                    };
                    //добавление экземпляра проекта в базу
                    db.Projects.Add(project);
                    db.SaveChanges();
                    //нахождение индекса текущего проекта
                }
                var idLast = db.Projects.Max(u => u.IDProject);
                if (indexProject != -1)
                    idLast = indexProject;
                foreach (var item in WorkingFieldCanvas.Children)
                {
                    //перебор элементов находящихся в рабочей области
                    if (item is Rectangle)
                    {
                        //нахождений растений в проекте
                        if ((item as Rectangle).StrokeThickness == 0)
                        {
                            string name = (item as Rectangle).Name.ToString();
                            //определение экземпляра растения
                            var query = db.Plants.FirstOrDefault(u => u.PlantName == name);
                            //добавление растений в проект
                            db.ProjectPlants.Add(new ProjectPlants()
                            {
                                IDProject = idLast,
                                IDPlant = query.IDPlant,
                                PointY = Canvas.GetTop(item as Rectangle),
                                PointX = Canvas.GetLeft(item as Rectangle)
                            });
                        }
                        //находжение строений в проекте
                        else if ((item as Rectangle).StrokeThickness == 1)
                        {
                            string name = (item as Rectangle).Name.ToString();
                            //определение экземпляра строения
                            var query = db.Structures.FirstOrDefault(u => u.StructureName == name);
                            //добавление строений в проект
                            db.ProjectStructure.Add(new ProjectStructure()
                            {
                                IDProject = idLast,
                                IDStructure = query.IDStructure,
                                WidthStructure = (item as Rectangle).Width / scale,
                                HeightStructure = (item as Rectangle).Height / scale,
                                PointY = Canvas.GetTop(item as Rectangle),
                                PointX = Canvas.GetLeft(item as Rectangle)
                            });
                        }
                        db.SaveChanges();
                    }
                    else continue;
                }
                string path = Directory.GetCurrentDirectory();
                //сохранение картинки проекта
                CreateSaveBitmap(WorkingFieldCanvas, path + "\\..\\..\\projectPicture\\" + TextBoxNameProject.Text + "#" + idLast + ".png");
                //сохранение состава проекта
                CreateSaveFile(idLast, path + "\\..\\..\\projectPicture\\" + TextBoxNameProject.Text + "#" + idLast + ".txt");
            }
        }

        //формирование файла с растениями в проекте
        private void CreateSaveFile(int idLast, string path)
        {
            using (LandscapeDesignEntities db = new LandscapeDesignEntities())
            {
                //поиск и подсчет количества растений в проекте
                var query = (from p in db.Projects
                            from pl in db.ProjectPlants
                            from plant in db.Plants
                            where pl.IDPlant == plant.IDPlant && p.IDProject == pl.IDProject && p.IDProject == idLast
                            group pl by plant.PlantName into plant
                            orderby plant.Count()
                            select (new { Name = plant.Key, Count = plant.Count() })).ToList();
                //добавление найденой информации в файл
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("*******В проекте используется*******");
                    foreach (var item in query)
                        sw.WriteLine($"{item.Name}: {item.Count} шт.");
                }
            }
        }

        //формирование картинки проекта
        private void CreateSaveBitmap(Canvas canvas, string filename)
        {
            //формирование картинки
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Pbgra32);
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
            renderBitmap.Render(canvas);
            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            //сохранен6ие картинки
            using (FileStream file = File.Create(filename))
                encoder.Save(file);
        }

        //изменение окон
        private void OldProjectOpen_Click(object sender, MouseButtonEventArgs e)
        {
            ShowRegistrationGrid(NewProjectWindow);
            ShowRegistrationGrid(OlpProjectWindow);
        }

        //работа с окном "существующие проекты"
        private void ButtonOldProject_Click(object sender, RoutedEventArgs e)
        {
            //если проект не выбран из списка
            if (ListProject.SelectedIndex == -1)
            {
                TextListErrorProject.Text = "Не коректный ввод размера";
                TextListErrorProject.Height = 30;
                TextListErrorProject.FontSize = 15;
            }
            //открытие проекта в рабочей области
            else
            {
                ShowRegistrationGrid(WindowNewProject);
                ShowRegistrationGrid(WorkingField);
                var elem = ListProject.SelectedItem;
                OpenOldProject((elem as ExplorerData).ID);
            }
        }

        //загрузка существующего проекта
        private void OpenOldProject(int iD)
        {
            using (LandscapeDesignEntities db = new LandscapeDesignEntities())
            {
                //поиск проекта с сообветствующим индексом
                foreach (var item in db.Projects)
                {
                    if (item.IDProject == iD)
                    {
                        scale = item.Scale;
                        //формирование рабочей области
                        WorkingFieldCanvas.Width = item.Width * scale;
                        WorkingFieldCanvas.Height = item.Height * scale;
                        InfoStatusBar.Width = WorkingFieldCanvas.Width;
                        scale = item.Scale;
                    }
                }
                //выборка нужных записей из базы
                var queryP = (from p in db.Projects
                            from pl in db.ProjectPlants
                            from plant in db.Plants
                            where p.IDProject == iD && p.IDProject==pl.IDProject && pl.IDPlant==plant.IDPlant
                            select(new {width = plant.WidthPlant, height = plant.HeightPlant,name = plant.PlantName,picture = plant.PicturePlantTop, x = pl.PointX, y = pl.PointY})).ToArray();

                string path = Directory.GetCurrentDirectory();
                //создание image "растение"
                foreach (var item in queryP)
                {
                    Rectangle rec = new Rectangle
                    {
                        Width = Convert.ToInt32(item.width) * scale / 10,
                        Height = Convert.ToInt32(item.height) * scale / 10,
                        Name = item.name,
                        StrokeThickness = 0,
                        Fill = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(path + "\\..\\..\\picture\\" + item.picture))
                        }
                    };
                    //отображение изображения на поле
                    WorkingFieldCanvas.Children.Add(rec);
                    Canvas.SetLeft(rec, item.x);
                    Canvas.SetTop(rec, item.y);
                };
                //выборка нужных записей из базы
                var queryS = (from s in db.Projects
                              from st in db.ProjectStructure
                              from struc in db.Structures
                              where s.IDProject == iD && s.IDProject == st.IDProject && st.IDStructure == struc.IDStructure
                              select (new { width = st.WidthStructure, height = st.HeightStructure, name = struc.StructureName, picture = struc.StructureTop, x = st.PointX, y = st.PointY })).ToArray();

                //создание image "строение"
                foreach (var item in queryS)
                {
                    Rectangle rec = new Rectangle
                    {
                        Width = Convert.ToInt32(item.width) * scale,
                        Height = Convert.ToInt32(item.height) * scale,
                        Name = item.name,
                        StrokeThickness = 1,
                        Fill = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(path + "\\..\\..\\picture\\" + item.picture))
                        }
                    };
                    //отображение изображения на поле
                    WorkingFieldCanvas.Children.Add(rec);
                    Canvas.SetLeft(rec, item.x);
                    Canvas.SetTop(rec, item.y);
                };
                //индекс текущего проекта
                indexProject = iD;
            }
        }
    }
}
