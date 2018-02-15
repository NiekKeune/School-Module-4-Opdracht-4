using System;
using System.Collections.Generic;
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
using GradesPrototype.Data;
using GradesPrototype.Services;

namespace GradesPrototype.Controls
{
    /// <summary>
    /// Interaction logic for AssignStudentDialog.xaml
    /// </summary>
    public partial class AssignStudentDialog : Window
    {
        public AssignStudentDialog()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            var unassignedStudents = from s in DataSource.Students      //#2 from the DataSource Students list
                                     where s.TeacherID == 0             //#3 where the TeacherID is 0 (meaning that these students don't have a teacher assigned to them).
                                     select s;                          //#1 Select the students

            if (unassignedStudents.Count() == 0)            //If there are no students without a teacher, the list wont show up.
            {
                txtMessage.Visibility = Visibility.Visible;
                list.Visibility = Visibility.Collapsed;
            }
            else                                            //If there are students without a teacher however, the list will show up.
            {
                txtMessage.Visibility = Visibility.Collapsed;
                list.Visibility = Visibility.Visible;

                list.ItemsSource = unassignedStudents;
            }
        }

        private void AssignStudentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Student_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button studentClicked = sender as Button;
                int studentID = (int)studentClicked.Tag;

                Student student = (from s in DataSource.Students        //#2 from the DataSource Students list
                                   where s.StudentID == studentID       //#3 where the ID matches.
                                   select s).First();                   //#1 Selects the student

                string message = String.Format("Do you wish to add {0} {1} to your class?", student.FirstName, student.LastName);
                MessageBoxResult reply = MessageBox.Show(message, "Confirm.", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (reply == MessageBoxResult.Yes)
                {
                    int teacherID = SessionContext.CurrentTeacher.TeacherID;            //Sets the teacherID to the Current Teacher's ID.
                    
                    SessionContext.CurrentTeacher.EnrollInClass(student);               //Student is added to the Current Teacher's class via the EnrollInClass method.

                    Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Couldn't enroll the student.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Close the dialog box
            this.Close();
        }
    }
}
