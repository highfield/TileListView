using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cet.UI.TileListView.Demo
{
    public class ViewModelBase
        : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>Occurs when a property value changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>Updates the property and raises the changed event, but only if the new value does not equal the old value. </summary>
        /// <param name="propertyName">The property name as lambda. </param>
        /// <param name="oldValue">A reference to the backing field of the property. </param>
        /// <param name="newValue">The new value. </param>
        /// <returns>True if the property has changed. </returns>
        public bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] String propertyName = null)
        {
            return Set(propertyName, ref oldValue, newValue);
        }


        /// <summary>Updates the property and raises the changed event, but only if the new value does not equal the old value. </summary>
        /// <param name="propertyName">The property name as lambda. </param>
        /// <param name="oldValue">A reference to the backing field of the property. </param>
        /// <param name="newValue">The new value. </param>
        /// <returns>True if the property has changed. </returns>
        public virtual bool Set<T>(String propertyName, ref T oldValue, T newValue)
        {
            if (Equals(oldValue, newValue))
                return false;

            oldValue = newValue;
            RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
            return true;
        }


        /// <summary>Raises the property changed event. </summary>
        /// <param name="propertyName">The property name. </param>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>Raises the property changed event. </summary>
        /// <param name="args">The arguments. </param>
        protected virtual void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var copy = PropertyChanged;
            if (copy != null)
                copy(this, args);
        }


        /// <summary>Raises the property changed event for all properties (string.Empty). </summary>
        public void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(string.Empty));
        }


        #region Validation

        private Dictionary<string, List<string>> _validationRegister;


        public void ValidateField(string propertyName)
        {
            this._validationRegister = this._validationRegister ?? new Dictionary<string, List<string>>();

            Action<string> validate = pn =>
            {
                //offre l'opportunità di validare il campo corrente alla pagina derivata
                var args = new ViewModelValidationErrorArgs(pn);
                this.OnValidating(args);

                this.RemoveError(pn);
                if (string.IsNullOrWhiteSpace(args.ErrorMessage))
                {
                    //this.RemoveError(pn);
                }
                else
                {
                    this.AddError(pn, args.ErrorMessage);
                }
            };

            //determina quali sono le proprietà coinvolte
            string[] names;
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                //tutte quelle esistenti
                names = this
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                    .Where(_ => _.DeclaringType != typeof(ViewModelBase))
                    .Select(_ => _.Name)
                    .ToArray();
            }
            else
            {
                //solo quella indicata
                names = new string[1] { propertyName };
            }

            for (int i = 0; i < names.Length; i++)
            {
                validate(names[i]);
            }
        }


        protected virtual void OnValidating(ViewModelValidationErrorArgs args)
        {
            var handler = this.Validating;
            if (handler != null)
            {
                handler(
                    this,
                    new ValidatingEventArgs(args)
                    );
            }
        }


        public void AddError(
            string propertyName,
            string error,
            bool prio = false
            )
        {
            if (this._validationRegister == null)
            {
                return;
            }

            List<string> collection;
            if (this._validationRegister.TryGetValue(propertyName, out collection) == false)
            {
                collection = new List<string>();
                this._validationRegister.Add(
                    propertyName,
                    collection
                    );
            }

            if (collection.Contains(error) == false)
            {
                if (prio)
                {
                    collection.Insert(0, error);
                }
                else
                {
                    collection.Add(error);
                }

                this.OnErrorsChanged(propertyName);
            }
        }


        public void RemoveError(string propertyName)
        {
            if (this._validationRegister == null)
            {
                return;
            }

            List<string> collection;
            if (this._validationRegister.TryGetValue(propertyName, out collection))
            {
                if (collection.Count != 0)
                {
                    //TEMP: lanciare eccezione o no?
                }

                this._validationRegister.Remove(propertyName);
                this.OnErrorsChanged(propertyName);
            }
        }


        public void RemoveError(
            string propertyName,
            string error
            )
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            if (this._validationRegister == null)
            {
                return;
            }

            List<string> collection;
            if (this._validationRegister.TryGetValue(propertyName, out collection) &&
                collection.Contains(error)
                )
            {
                collection.Remove(error);
                if (collection.Count == 0)
                {
                    this._validationRegister.Remove(propertyName);
                }

                this.OnErrorsChanged(propertyName);
            }
        }


        public bool HasErrors
        {
            get { return this._validationRegister?.Count > 0; }
        }


        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (this._validationRegister != null)
            {
                if (propertyName == "*")
                {
                    return this._validationRegister
                        .SelectMany(_ => _.Value)
                        .Where(_ => string.IsNullOrEmpty(_) == false)
                        .ToList();
                }
                else if (
                    string.IsNullOrEmpty(propertyName) ||
                    this._validationRegister.ContainsKey(propertyName) == false
                    )
                {
                    return null;
                }

                return this._validationRegister[propertyName];
            }
            else
            {
                return new string[0];
            }
        }


        #region EVT ErrorsChanged

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        protected virtual void OnErrorsChanged(string propertyName)
        {
            var handler = this.ErrorsChanged;

            if (handler != null)
            {
                handler(
                    this,
                    new DataErrorsChangedEventArgs(propertyName)
                    );
            }
        }

        #endregion


        #region EVT Validating

        public event EventHandler<ValidatingEventArgs> Validating;

        #endregion

        #endregion


        #region Editing

        #region PROP IsLoaded

        private bool _isLoaded;

        public bool IsLoaded
        {
            get { return this._isLoaded; }
            private set { Set(ref this._isLoaded, value); }
        }

        #endregion


        #region PROP IsModified

        private bool _isModified;

        public bool IsModified
        {
            get { return this._isModified; }
            set { Set(ref this._isModified, value); }
        }

        #endregion


        public Task<bool> LoadAsync()
        {
            return this.LoadAsync(null, CancellationToken.None);
        }


        public async Task<bool> LoadAsync(
            object context,
            CancellationToken token
            )
        {
            this.IsLoaded = false;
            bool success = await this.OnLoadAsync(context, token);
            this.IsModified = false;
            this.IsLoaded = true;
            return success;
        }


        protected virtual Task<bool> OnLoadAsync(
            object context,
            CancellationToken token
            )
        {
            return Task.FromResult(true);
        }


        public Task<bool> SaveAsync()
        {
            return this.SaveAsync(null, CancellationToken.None);
        }


        public async Task<bool> SaveAsync(
            object context,
            CancellationToken token
            )
        {
            bool success = await this.OnSaveAsync(context, token);
            this.IsModified = false;
            return success;
        }


        protected virtual Task<bool> OnSaveAsync(
            object context,
            CancellationToken token
            )
        {
            return Task.FromResult(true);
        }

        #endregion

    }


    public sealed class ViewModelValidationErrorArgs
    {
        public ViewModelValidationErrorArgs(string name)
        {
            this.FieldName = name;
        }

        public string FieldName { get; private set; }
        public string ErrorMessage { get; set; }
    }


    public sealed class ValidatingEventArgs
        : EventArgs
    {
        public ValidatingEventArgs(ViewModelValidationErrorArgs info)
        {
            this.Info = info;
        }

        public ViewModelValidationErrorArgs Info { get; private set; }
    }
}
