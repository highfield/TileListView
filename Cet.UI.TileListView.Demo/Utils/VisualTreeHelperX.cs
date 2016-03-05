using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Cet.UI
{
    public static class VisualTreeHelperX
    {

        #region Scansione verso l'alto (antenati)

        /// <summary>
        /// Cerca il primo elemento visuale genitore che e' caratterizzato dal nome indicato a partire dall'oggetto specificato incluso
        /// </summary>
        /// <param name="source">Il riferimento all'oggetto dal quale iniziare la ricerca</param>
        /// <param name="name">Il nome dell'elemento cercato</param>
        /// <returns>L'elemento cercato, se la ricerca ha avuto successo, altrimenti null</returns>
        /// <remarks>La ricerca include l'oggetto di partenza</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        /**************************************************************************************************
         * Soppressione del warning CA1004 perche' in questo caso l'utilizzo dei generici e' lecito.
         * Vedi: http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/d703fc0d-321e-4171-81c2-52f48701672b
         **************************************************************************************************/
        public static T FindAncestorOrSelfByName<T>(
            DependencyObject source,
            string name
            )
            where T : FrameworkElement
        {
            foreach (var obj in VisualTreeHelperX.GetAncestry(source))
            {
                var target = obj as T;
                if (target != null && target.Name == name)
                {
                    return target;
                }
            }

            return (T)null;
        }


        /// <summary>
        /// Cerca il primo elemento visuale genitore che e' caratterizzato dal nome indicato a partire dall'oggetto specificato
        /// </summary>
        /// <param name="source">Il riferimento all'oggetto dal quale iniziare la ricerca</param>
        /// <param name="name">Il nome dell'elemento cercato</param>
        /// <returns>L'elemento cercato, se la ricerca ha avuto successo, altrimenti null</returns>
        /// <remarks>La ricerca non prende in considerazione l'oggetto di partenza</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        /**************************************************************************************************
         * Soppressione del warning CA1004 perche' in questo caso l'utilizzo dei generici e' lecito.
         * Vedi: http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/d703fc0d-321e-4171-81c2-52f48701672b
         **************************************************************************************************/
        public static T FindAncestorByName<T>(
            DependencyObject source,
            string name
            )
            where T : FrameworkElement
        {
            if (source != null)
            {
                //ricava il genitore visuale dell'oggetto corrente
                source = VisualTreeHelper.GetParent(source);

                foreach (var obj in VisualTreeHelperX.GetAncestry(source))
                {
                    var target = obj as T;
                    if (target != null && target.Name == name)
                    {
                        return target;
                    }
                }
            }

            return (T)null;
        }


        /// <summary>
        /// Cerca il primo genitore visuale di tipo T a partire dall'oggetto specificato incluso
        /// </summary>
        /// <typeparam name="T">Il tipo di oggetto cercato</typeparam>
        /// <param name="source">Il riferimento all'oggetto dal quale iniziare la ricerca</param>
        /// <returns>L'oggetto gerarchicamente superiore di tipo T, se la ricerca ha avuto successo, altrimenti null</returns>
        /// <remarks>La ricerca include l'oggetto di partenza</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        /**************************************************************************************************
         * Soppressione del warning CA1004 perche' in questo caso l'utilizzo dei generici e' lecito.
         * Vedi: http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/d703fc0d-321e-4171-81c2-52f48701672b
         **************************************************************************************************/
        public static T FindAncestorOrSelfByType<T>(
            DependencyObject source
            )
            where T : DependencyObject
        {
            foreach (var obj in VisualTreeHelperX.GetAncestry(source))
            {
                var target = obj as T;
                if (target != null)
                {
                    return target;
                }
            }

            return (T)null;
        }


        /// <summary>
        /// Cerca il primo genitore visuale di tipo T a partire dall'oggetto specificato
        /// </summary>
        /// <typeparam name="T">Il tipo di oggetto cercato</typeparam>
        /// <param name="source">Il riferimento all'oggetto dal quale iniziare la ricerca</param>
        /// <returns>L'oggetto gerarchicamente superiore di tipo T, se la ricerca ha avuto successo, altrimenti null</returns>
        /// <remarks>La ricerca non prende in considerazione l'oggetto di partenza</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        /**************************************************************************************************
         * Soppressione del warning CA1004 perche' in questo caso l'utilizzo dei generici e' lecito.
         * Vedi: http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/d703fc0d-321e-4171-81c2-52f48701672b
         **************************************************************************************************/
        public static T FindAncestorByType<T>(
            DependencyObject source
            )
            where T : DependencyObject
        {
            if (source != null)
            {
                //ricava il genitore visuale dell'oggetto corrente
                source = VisualTreeHelper.GetParent(source);

                foreach (var obj in VisualTreeHelperX.GetAncestry(source))
                {
                    var target = obj as T;
                    if (target != null)
                    {
                        return target;
                    }
                }
            }

            return (T)null;
        }


        /// <summary>
        /// Restituisce un'enumerazione con l'intera gerarchia visuale a partire dall'oggetto specificato
        /// </summary>
        /// <param name="source">L'oggetto dal quale iniziare la scansione</param>
        /// <returns>L'enumerazione che parte dall'oggetto specificato e via via va verso l'alto</returns>
        /// <remarks>
        /// L'enumerazione include l'oggetto sorgente.
        /// Se l'oggetto e' nullo viene restituita un'enumerazione vuota.
        /// </remarks>
        public static IEnumerable<DependencyObject> GetAncestry(
            DependencyObject source
            )
        {
            //percorre la gerarchia verso l'alto
            while (source != null)
            {
                yield return source;

                //ricava il genitore visuale dell'oggetto corrente
                source = VisualTreeHelper.GetParent(source);
            }
        }


        /// <summary>
        /// Restituisce il nodo-radice (capostipite) dell'albero visuale del quale
        /// l'elemento specificato fa parte
        /// </summary>
        /// <param name="source">L'elemento dal quale risalire la gerarchia.</param>
        /// <returns>Il nodo-radice dell'albero visuale</returns>
        public static DependencyObject GetRoot(DependencyObject source)
        {
            DependencyObject root = null;

            while (source != null)
            {
                //ricava il genitore visuale dell'oggetto corrente
                root = source;
                source = VisualTreeHelper.GetParent(source);
            }

            return root;
        }

        #endregion


        #region Scansione verso il basso (discendenze)

        /// <summary>
        /// Cerca il primo elemento visuale discendente che e' caratterizzato dal nome indicato
        /// </summary>
        /// <param name="source">Il riferimento all'oggetto dal quale iniziare la ricerca</param>
        /// <param name="name">Il nome dell'elemento cercato</param>
        /// <returns>L'elemento cercato, se la ricerca ha avuto successo, altrimenti null</returns>
        /// <remarks>
        /// La ricerca non prende in considerazione l'oggetto di partenza.
        /// In questa versione viene proposta una profondita' massima pari a 10 livelli.
        /// Per ulteriori dettagli vedi <see cref="FindChild"/>.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        /**************************************************************************************************
         * Soppressione del warning CA1004 perche' in questo caso l'utilizzo dei generici e' lecito.
         * Vedi: http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/d703fc0d-321e-4171-81c2-52f48701672b
         **************************************************************************************************/
        public static T FindChildByName<T>(
            DependencyObject source,
            string name)
            where T : FrameworkElement
        {
            return VisualTreeHelperX.FindChildByName<T>(
                source,
                name,
                10) as T;
        }


        /// <summary>
        /// Cerca il primo elemento visuale discendente che e' caratterizzato dal nome indicato
        /// </summary>
        /// <param name="source">Il riferimento all'oggetto dal quale iniziare la ricerca</param>
        /// <param name="name">Il nome dell'elemento cercato</param>
        /// <param name="maxDepth">La massima profondita' da raggiungere durante la ricerca</param>
        /// <returns>L'elemento cercato, se la ricerca ha avuto successo, altrimenti null</returns>
        /// <remarks>
        /// La ricerca non prende in considerazione l'oggetto di partenza.
        /// Per ulteriori dettagli vedi <see cref="FindChild"/>.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        /**************************************************************************************************
         * Soppressione del warning CA1004 perche' in questo caso l'utilizzo dei generici e' lecito.
         * Vedi: http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/d703fc0d-321e-4171-81c2-52f48701672b
         **************************************************************************************************/
        public static T FindChildByName<T>(
            DependencyObject source,
            string name,
            int maxDepth)
            where T : FrameworkElement
        {
            Predicate<DependencyObject> predicate = obj =>
            {
                var element = obj as FrameworkElement;
                return (element != null && element.Name == name);
            };

            return VisualTreeHelperX.FindChild(
                source,
                predicate,
                maxDepth) as T;
        }


        /// <summary>
        /// Cerca il primo discendente visuale di tipo T a partire dall'oggetto specificato
        /// </summary>
        /// <typeparam name="T">Il tipo di oggetto cercato</typeparam>
        /// <param name="source">Il riferimento all'oggetto dal quale iniziare la ricerca</param>
        /// <returns>L'oggetto discendente di tipo T, se la ricerca ha avuto successo, altrimenti null</returns>
        /// <remarks>
        /// La ricerca non prende in considerazione l'oggetto di partenza.
        /// In questa versione viene proposta una profondita' massima pari a 10 livelli.
        /// Per ulteriori dettagli vedi <see cref="FindChild"/>.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        /**************************************************************************************************
         * Soppressione del warning CA1004 perche' in questo caso l'utilizzo dei generici e' lecito.
         * Vedi: http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/d703fc0d-321e-4171-81c2-52f48701672b
         **************************************************************************************************/
        public static T FindChildByType<T>(DependencyObject source)
            where T : DependencyObject
        {
            return VisualTreeHelperX.FindChildByType<T>(
                source,
                10);
        }


        /// <summary>
        /// Cerca il primo discendente visuale di tipo T a partire dall'oggetto specificato
        /// </summary>
        /// <typeparam name="T">Il tipo di oggetto cercato</typeparam>
        /// <param name="source">Il riferimento all'oggetto dal quale iniziare la ricerca</param>
        /// <param name="maxDepth">La massima profondita' da raggiungere durante la ricerca</param>
        /// <returns>L'oggetto discendente di tipo T, se la ricerca ha avuto successo, altrimenti null</returns>
        /// <remarks>
        /// La ricerca non prende in considerazione l'oggetto di partenza.
        /// Per ulteriori dettagli vedi <see cref="FindChild"/>.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        /**************************************************************************************************
         * Soppressione del warning CA1004 perche' in questo caso l'utilizzo dei generici e' lecito.
         * Vedi: http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/d703fc0d-321e-4171-81c2-52f48701672b
         **************************************************************************************************/
        public static T FindChildByType<T>(
            DependencyObject source,
            int maxDepth)
            where T : DependencyObject
        {
            if (source == null)
            {
                throw new ArgumentNullException(
                    "source",
                    "The source parameter cannot be null");
            }

            return (T)VisualTreeHelperX.FindChild(
                source,
                obj => obj is T,
                maxDepth);
        }


        /// <summary>
        /// Ricerca il discendente visuale che soddisfa al predicato
        /// </summary>
        /// <param name="source">L'oggetto dal quale iniziare la scansione</param>
        /// <param name="predicate">Il predicato che indica quando la ricerca ha esaurito il suo compito</param>
        /// <param name="maxDepth">La massima profondita' da raggiungere durante la ricerca</param>
        /// <returns>Il discendente cercato oppure "null"</returns>
        /// <remarks>
        /// La ricerca non prende in considerazione l'oggetto di partenza.
        /// La scansione dell'albero e' di tipo Depth-first, a profondita' limitata.
        /// Anche se non appare come il moglior metodo per scandire i nodi dell'albero,
        /// ha diversi vantaggi rispetto ad algoritmi alternativi: semplicita', efficienza,
        /// scarso uso della memoria, ecc.
        /// </remarks>
        public static DependencyObject FindChild(
            DependencyObject source,
            Predicate<DependencyObject> predicate,
            int maxDepth)
        {
            if (source == null)
            {
                throw new ArgumentNullException(
                    "source",
                    "The source parameter cannot be null");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(
                    "predicate",
                    "The predicate cannot be null");
            }


            DependencyObject result = null;

            //valuta i figli del nodo sorgente
            var children = VisualTreeHelperX.GetChildren(source).ToList();

            foreach (var child in children)
            {
                //valuta se il nodo corrente soddisfa il predicato
                if (predicate(child))
                {
                    result = child;
                    break;
                }
            }

            if (result == null && maxDepth > 1)
            {
                //prova piu' in profondita'
                foreach (var child in children)
                {
                    result = VisualTreeHelperX.FindChild(
                        child,
                        predicate,
                        maxDepth - 1);

                    if (result != null)
                        break;
                }
            }

            return result;
        }


        /// <summary>
        /// Restituisce un'enumerazione con tutti i figli visuali dell'oggetto dato
        /// </summary>
        /// <param name="source">L'oggetto di riferimento</param>
        /// <returns>L'enumerazione con i figli visuali</returns>
        public static IEnumerable<DependencyObject> GetChildren(DependencyObject source)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(source); i++)
            {
                yield return VisualTreeHelper.GetChild(source, i);
            }
        }


        /// <summary>
        /// Restituisce un'enumerazione con tutti i discendenti dell'oggetto dato
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetDescendantAndSelf(DependencyObject source)
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                yield return item;

                foreach (var child in VisualTreeHelperX.GetChildren(item))
                {
                    queue.Enqueue(child);
                }
            }
        }

        #endregion

    }
}
