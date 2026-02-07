using JobOverview.Entities;

namespace JobOverview.entities
{

   public class Service
   {
      public string Code { get; set; } = "";
      public string Nom { get; set; } = "";
   }
   public class Equipe
   {
      public string Code { get; set; } = string.Empty;
      public string Nom { get; set; } = string.Empty;

      public string CodeService{ get; set; } = string.Empty;
      public string CodeFiliere { get; set; } = string.Empty;

      //prop de navigation pour recuperer la liste d'une equipe GetEquipes (mettre aussi dans dbcontext)
      public virtual List<Personne> Personnes { get; set; } = new();
      //public virtual Service Service { get; set; } = null!; // pour recuperer le de GetPersonne
        public virtual Service Service { get; set; } = new(); // pour ne pas creer de nouveau service //on les remettre à null
        // dans la methode de service
    }

   public class Personne
   {
      public string Pseudo { get; set; } = string.Empty;
      public string Nom { get; set; } = string.Empty;
      public string Prenom { get; set; } = string.Empty;
      public decimal TauxProductivite { get; set; }
      public string CodeEquipe { get; set; } = string.Empty;
      public string CodeMetier { get; set; } = string.Empty;
      public string? Manager { get; set; } 

      //prop de navigation
      //public virtual Metier Metier { get; set; } = null!; // pour recuperer les metier dans GEt personnne

        //prop de navigation pour reucperer la pers de pseudo donné avec son metier et ses activités
        // api/Personnes/RBEAUMONT
        public virtual Metier Metier { get; set; } = new(); // pour ne pas creer de nouveau metier 
    }

  

   public class Metier
   {
      public string Code { get; set; } = string.Empty;
      public string Titre { get; set; } = string.Empty;

      public string CodeService { get; set; } = string.Empty;

        //prop de navigation pour recuperer la liste des metiers d'un service
        public virtual List<Activite> Activites { get; set; } = new();
    }
}
