using ExpressionParser.Data;
using ExpressionParser.Parser;
using ExpressionParser.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser.Controllers
{
    [Route("expression")]
    [ApiController]
    public class ExpressionController : ControllerBase
    {
        private readonly IExpressionRepo _repository;
        private readonly ILogger<ExpressionController> _logger;

        public ExpressionController(IExpressionRepo repository, ILogger<ExpressionController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        //[HttpPost("get")][HttpGet("{id:length(24)}", Name = "GetProduct")]
        [HttpGet("{encodedExpression}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Expression), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Expression>> GetExpression(string encodedExpression)
        {

            byte[] decodedExpressionBytes = System.Convert.FromBase64String(encodedExpression);
            string decodedExpression = System.Text.Encoding.UTF8.GetString(decodedExpressionBytes);
            var tmp = await _repository.GetExpression(decodedExpression);
            if (tmp == null)
            {
                _logger.LogInformation($"Expresssion with expresie: {decodedExpression}, hasn't been found in database.");
                double result;

                if (decodedExpression.ToLower().Contains('x'))
                {
                    result = EqParser.EvaluateEquation(decodedExpression);
                }
                else
                {
                    result = EqParser.EvaluateExprTokens(EqParser.ConvertToPostfixToken(decodedExpression));
                };
                //List<Token> postfix = EqParser.ConvertToPostfixToken(expression);
                //double result = EqParser.EvaluateExprTokens(postfix);
                Expression ret = new Expression { Expresie = decodedExpression, Result = result.ToString() };
                await _repository.AddExpression(ret);
                return Ok(ret);

            }
            return Ok(tmp);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Expression), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> AddExpression([FromBody] Expression expr)
        {
            await _repository.AddExpression(expr);
            return CreatedAtAction("GetExpression", new { encodedExpression = Convert.ToBase64String(Encoding.ASCII.GetBytes(expr.Expresie)) }, expr);
        }


        [HttpGet("test/{encodedExpression}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Expression), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Expression>> Test(string encodedExpression)
        {

            try
            {
                byte[] decodedExpressionBytes = System.Convert.FromBase64String(encodedExpression);

                string expression = System.Text.Encoding.UTF8.GetString(decodedExpressionBytes);
                double result;

                if (expression.ToLower().Contains('x'))
                {
                    result = EqParser.EvaluateEquation(expression);
                }
                else
                {
                    result = EqParser.EvaluateExprTokens(EqParser.ConvertToPostfixToken(expression));
                }

                var ex = Token.StringToTokens(expression);
                //List<Token> postfix = EqParser.ConvertToPostfixToken(expression);
                //double result = EqParser.EvaluateExprTokens(postfix);
                string postfixRex = "";
                foreach (var t in ex)
                {
                    postfixRex += t;
                }
                Expression ret = new Expression { Expresie = postfixRex, Result = result.ToString() };
                return Ok(ret);
            }
            catch (Exception e)
            {
                return BadRequest();
            };


        }
    }
}
