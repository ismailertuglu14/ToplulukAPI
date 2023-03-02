using System;
using System.Collections.Generic;

namespace Topluluk.Shared.Dtos
{
	public class DatabaseResponse
	{
        public bool IsSuccess { get; set; }
        public dynamic Data { get; set; }
        public List<string> ErrorMessage { get; set; } = new List<string>();
	}
}

