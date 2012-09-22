using HelloWorld.Authors;
using HelloWorld.Books;
using HelloWorld.Books.Classification;

namespace HelloWorld
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IRepository<Book>>().Singleton().Use(Repository<Book>.With(
                new Book { Name = "QED: The Strange Theory of Light and Matter (Princeton Science Library)", Author = "Richard P. Feynman", Published = 2006, Isbn = "0691125759" },
                new Book { Name = "From c-Numbers to q-Numbers: The Classical Analogy in the History of Quantum Theory (California Studies in the History of Science)", Author = "Olivier Darrigol", Published = 1993, Isbn = "0520078225" },
                new Book { Name = "The Principia : Mathematical Principles of Natural Philosophy", Author = "Isaac Newton", Published = 1999, Isbn = "0520088174" },
                new Book { Name = "Classic Feynman: All the Adventures of a Curious Character", Author = "Richard P. Feynman", Published = 2005, Isbn = "0393061329" },
                new Book { Name = "Encounters with Einstein", Author = "Werner Heisenberg", Published = 1989, Isbn = "0691024332" },
                new Book { Name = "Physics and Beyond: Encounters and Conversations", Author = "Werner Heisenberg", Published = 1971, Isbn = "0061316229" },
                new Book { Name = "Physics and Philosophy: The Revolution in Modern Science", Author = "Werner Heisenberg", Published = 2007, Isbn = "0061209198" },
                new Book { Name = "Philosophical Problems of Quantum Physics", Author = "Werner Heisenberg", Published = 1979, Isbn = "0918024153" },
                new Book { Name = "Perfectly Reasonable Deviations from the Beaten Track: The Letters of Richard P. Feynman", Author = "Richard P. Feynman", Published = 2005, Isbn = "0738206369" },
                new Book { Name = "The Pleasure of Finding Things Out: The Best Short Works of Richard P. Feynman (Helix Books)", Author = "Richard P. Feynman", Published = 2005, Isbn = "0465023959" },
                new Book { Name = "Einstein's Miraculous Year: Five Papers That Changed the Face of Physics", Author = "Albert Einstein", Published = 2005, Isbn = "0691122288" },
                new Book { Name = "Fritz London: A Scientific Biography", Author = "Kostas Gavroglu", Published = 2005, Isbn = "052102319X" },
                new Book { Name = "Quantum Generations: A History of Physics in the Twentieth Century", Author = "Helge Kragh", Published = 2002, Isbn = "0691095523" },
                new Book { Name = "Physics in the Nineteenth Century", Author = "Robert D. Purrington", Published = 1997, Isbn = "0813524423" },
                new Book { Name = "Subtle Is the Lord: The Science and the Life of Albert Einstein", Author = "Abraham Pais", Published = 2005, Isbn = "0192806726" },
                new Book { Name = "The Structure of Scientific Revolutions", Author = "Thomas S. Kuhn", Published = 1996, Isbn = "0226458083" },
                new Book { Name = "Volume III - Essays 1958-1962 on Atomic Physics and Human Knowledge", Author = "Niels Bohr", Published = 1995, Isbn = "0918024544" },
                new Book { Name = "Volume I - Atomic Theory and the Description of Nature (Philosophical Writings of Niels Bohr Series, Vol 1)", Author = "Niels Bohr", Published = 1987, Isbn = "0918024501" },
                new Book { Name = "Essays 1933 to 1957 on Atomic Physics and Human Knowledge, Vol. 2", Author = "Niels Bohr", Published = 1987, Isbn = "0918024528" },
                new Book { Name = "Volume IV - Causality and Complementarity", Author = "Niels Henrik David Bohr", Published = 1999, Isbn = "9781881987130" },
                new Book { Name = "The Princeton Companion to Mathematics", Author = "Timothy Gowers", Published = 2008, Isbn = "0691118809" },
                new Book { Name = "Black-Body Theory and the Quantum Discontinuity, 1894-1912", Author = "Thomas S. Kuhn", Published = 1987, Isbn = "0226458008" },
                new Book { Name = "Order, Chaos Order: The Transition from Classical to Quantum Physics", Author = "Philip Stehle", Published = 1994, Isbn = "019508473X" },
                new Book { Name = "Euclid's Elements", Author = "Euclid", Published = 2002, Isbn = "1888009195" },
                new Book { Name = "The Oxford Companion to the History of Modern Science (Oxford Companions Ncs)", Author = "John L. Heilbron", Published = 2003, Isbn = "0195112296" }));

            For<IRepository<Author>>().Singleton().Use(Repository<Author>.With(
                new Author { Name = "Albert Einstein" },
                new Author { Name = "Don Syme" },
                new Author { Name = "Euclid" },
                new Author { Name = "F. David Peat" },
                new Author { Name = "Freeman J. Dyson" },
                new Author { Name = "George Gamow" },
                new Author { Name = "George Orwell" },
                new Author { Name = "Helge Kragh" },
                new Author { Name = "Isaac Asimov" },
                new Author { Name = "Isaac Newton" },
                new Author { Name = "Kevin D. Mitnick" },
                new Author { Name = "Niels Bohr" },
                new Author { Name = "Olivier Darrigol" },
                new Author { Name = "Richard P. Feynman" },
                new Author { Name = "Steve Freeman" },
                new Author { Name = "Steven Weinberg" },
                new Author { Name = "Thomas S. Kuhn" },
                new Author { Name = "Werner Heisenberg" },
                new Author { Name = "Will & Ariel Durant" },
                new Author { Name = "William F Magie" }));

            For<IRepository<Category>>().Singleton().Use(Repository<Category>.With(
                new Category { Name = "Electrons" },
                new Category { Name = "Photons" },
                new Category { Name = "Quantum electrodynamics" },
                new Category { Name = "Particles (Nuclear physics)" },
                new Category { Name = "Light, Wave theory of" },
                new Category { Name = "Physics" },
                new Category { Name = "Electromagnetism" },
                new Category { Name = "Quantum theory" },
                new Category { Name = "Heisenberg uncertainty principle" },
                new Category { Name = "Blackbody radiation" },
                new Category { Name = "Thermodynamics" },
                new Category { Name = "Complementarity (Physics)" },
                new Category { Name = "Causality (Physics)" },
                new Category { Name = "Radioactivity" },
                new Category { Name = "Nuclear physicists" },
                new Category { Name = "Nuclear fission" },
                new Category { Name = "Nuclear energy" },
                new Category { Name = "Nuclear physics" }));
        }
    }
}