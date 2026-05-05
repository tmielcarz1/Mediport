using System.Text.Json.Serialization;

namespace WebApplication1.Application.Models.Common
{
    public class State
    {
        #region Properties

        /// <summary>
        /// Is not valid ?
        /// </summary>
        [JsonIgnore]
        public bool IsNotValid => Errors.Any();

        /// <summary>
        /// Is valid?
        /// </summary>
        [JsonPropertyName("result")]
        public bool IsValid => !IsNotValid;

        /// <summary>
        /// For Error 204
        /// </summary>
        [JsonPropertyName("IsPartiallyValid")]
        public bool IsPartiallyValid = false;


        /// <summary>
        /// Returned object
        /// </summary>
        [JsonPropertyName("payload")]
        public object StateObject { get; set; }

        /// <summary>
        /// Errors
        /// </summary>
        [JsonPropertyName("errors_debug")]
        public List<string> Errors { get; set; }
        /// <summary>
        /// Errors Dictionary
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, List<string>> ErrorsDictionary { get; set; }
        /// <summary>
        /// Messages
        /// </summary>
        [JsonIgnore]
        public List<string> Messages { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// State constructor
        /// </summary>
        public State()
        {
            Errors = new List<string>();
            Messages = new List<string>();
            ErrorsDictionary = new Dictionary<string, List<string>>();
        }


        /// <summary>
        /// State constructor
        /// </summary>
        public State(Dictionary<string, List<string>> errorsDictionary)
        {
            Errors = errorsDictionary.Values.SelectMany(x => x).ToList();
            Messages = new List<string>();
            ErrorsDictionary = errorsDictionary;
        }

        #endregion Constructors

        #region Exception methods

        /// <summary>
        /// Method to add exception msg
        /// </summary>
        /// <param name="e">Exception</param>
        public void AddException(Exception e)
        {
            if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
            {
                Errors.Add(e.InnerException.Message);
            }
            if (!string.IsNullOrEmpty(e.Message))
            {
                Errors.Add(e.Message);
            }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="err"></param>
        public void AddError(string err)
        {
            if (!Errors.Contains(err))
            {
                Errors.Add(err);
                AddToDictionary("", err);
            }
        }

        /// <summary>
        /// Add error dictonary
        /// </summary>
        /// <param name="err"></param>
        public void AddError(string key, string err)
        {
            AddToDictionary(key, err);

            if (!Errors.Contains(err))
                Errors.Add(err);
        }

        private void AddToDictionary(string key, string err)
        {
            if (ErrorsDictionary.ContainsKey(key))
            {
                ErrorsDictionary[key].Add(err);
            }
            else
            {
                ErrorsDictionary.Add(key, new List<string>() { err });
            }
        }
        /// <summary>
        /// Add message
        /// </summary>
        /// <param name="msg"></param>
        public void AddMessage(string msg)
        {
            Messages.Add(msg);
        }

        #endregion Exception methods

        #region Check methods

        /// <summary>
        /// Check state of operation if false set error in RepoState
        /// </summary>
        /// <param name="state">State of function</param>
        /// <param name="errorMessage">Error message</param>
        public void ExecuteStatus(bool state, string errorMessage)
        {
            if (!state)
            {
                AddError(errorMessage);
            }
        }

        #endregion Check methods

        /// <summary>
        /// Gets errors
        /// </summary>
        /// <returns></returns>
        #region OtherMethods
        public string GetErrors()
        {
            return string.Join("\n", Errors);
        }

        /// <summary>
        /// Gets messages
        /// </summary>
        /// <returns></returns>
        public string GetMessages()
        {
            return string.Join("\n", Messages);
        }
        #endregion
    }




    public class State<TResult>
    {
        #region Properties



        /// <summary>
        /// Is not valid ?
        /// </summary>
        [JsonIgnore]
        public bool IsNotValid => Errors.Any();

        /// <summary>
        /// Is valid?
        /// </summary>
        [JsonPropertyName("result")]
        public bool IsValid => !IsNotValid;


        /// <summary>
        /// Returned object
        /// </summary>
        [JsonPropertyName("payload")]
        public TResult StateObject { get; set; }

        /// <summary>
        /// Errors
        /// </summary>
        [JsonPropertyName("errors_debug")]
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Errors Dictionary
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, List<string>> ErrorsDictionary { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Messages
        /// </summary>
        [JsonIgnore]
        public List<string> Messages { get; set; } = new List<string>();

        #endregion Properties

        #region Exception methods

        /// <summary>
        /// Method to add exception msg
        /// </summary>
        /// <param name="e">Exception</param>
        public void AddException(Exception e)
        {
            if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
            {
                Errors.Add(e.InnerException.Message);
            }
            if (!string.IsNullOrEmpty(e.Message))
            {
                Errors.Add(e.Message);
            }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="err"></param>
        public void AddError(string err)
        {
            if (!Errors.Contains(err))
            {
                Errors.Add(err);
                AddToDictionary("", err);
            }
        }

        /// <summary>
        /// Add error dictonary
        /// </summary>
        /// <param name="err"></param>
        public void AddError(string key, string err)
        {
            AddToDictionary(key, err);

            if (!Errors.Contains(err))
                Errors.Add(err);
        }

        private void AddToDictionary(string key, string err)
        {
            if (ErrorsDictionary.ContainsKey(key))
            {
                ErrorsDictionary[key].Add(err);
            }
            else
            {
                ErrorsDictionary.Add(key, new List<string>() { err });
            }
        }
        /// <summary>
        /// Add errors
        /// </summary>
        /// <param name="err"></param>
        public void AddErrors(List<string> errs)
        {
            Errors.AddRange(errs);
            if (ErrorsDictionary.ContainsKey(""))
            {
                ErrorsDictionary[""].AddRange(errs);
            }
            else
            {
                ErrorsDictionary.Add("", new List<string>(errs));
            }
        }

        /// <summary>
        /// Add message
        /// </summary>
        /// <param name="msg"></param>
        public void AddMessage(string msg)
        {
            Messages.Add(msg);
        }

        #endregion Exception methods

        #region Check methods

        /// <summary>
        /// Check state of operation if false set error in RepoState
        /// </summary>
        /// <param name="state">State of function</param>
        /// <param name="errorMessage">Error message</param>
        public void ExecuteStatus(bool state, string errorMessage)
        {
            if (!state)
            {
                AddError(errorMessage);
            }
        }

        #endregion Check methods

        /// <summary>
        /// Gets errors
        /// </summary>
        /// <returns></returns>
        #region OtherMethods
        public string GetErrors()
        {
            return string.Join("\n", Errors.Distinct());
        }

        /// <summary>
        /// Gets messages
        /// </summary>
        /// <returns></returns>
        public string GetMessages()
        {
            return string.Join("\n", Messages);
        }
        #endregion
    }
}
