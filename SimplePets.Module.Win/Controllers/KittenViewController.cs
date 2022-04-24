using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using SimplePets.Module.BusinessObjects;
using System;
using System.Linq;

namespace SimplePets.Module.Win.Controllers
{
    public class KittenViewController : ViewController
    {
        SimpleAction actionAddKittenEF;
        SimpleAction actAddKittenXAF;
        public KittenViewController() : base()
        {
            TargetObjectType = typeof(Kitten);
            TargetViewNesting = Nesting.Nested;
            actAddKittenXAF = new SimpleAction(this, "Add via OS", "View");
            actAddKittenXAF.Execute += actAddKittenXAF_Execute;

            actionAddKittenEF = new SimpleAction(this, "Add via Db", "View");
            actionAddKittenEF.Execute += actionAddKittenEF_Execute;

        }
        private void actionAddKittenEF_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var cat = View.ObjectSpace.GetObject(((NestedFrame)Frame).ViewItem.CurrentObject) as Cat;
            var db = Helpers.MakeDb();
            var kitten = new Kitten
            {
                Parent = db.Cats.FirstOrDefault(c => c.Id == cat.Id),
                Name = $"baby {cat.Kittens.Count + 1} of {cat.Name}"
            };
            db.Kittens.Add(kitten);
            db.SaveChanges();
            View.ObjectSpace.Refresh();



        }
        private void actAddKittenXAF_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            var cat = View.ObjectSpace.GetObject(((NestedFrame)Frame).ViewItem.CurrentObject) as Cat;
            var os = View.ObjectSpace;
            var kitten = os.CreateObject<Kitten>();
            kitten.Parent = cat;
            kitten.Name = $"baby {cat.Kittens.Count + 1} of {cat.Name}";
            View.ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
    }

}
