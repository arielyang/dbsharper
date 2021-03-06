﻿<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns:script="urn:scripts">
<xsl:import href="DbSharper.DataAccess.ExecuteHeader.xslt" />
<xsl:import href="DbSharper.DataAccess.ExecuteParameters.xslt" />
<xsl:import href="DbSharper.DataAccess.ExecuteOutputParameters.xslt" />
<xsl:template name="ExecuteReader" match="/">
<xsl:variable name="cacheKey" select="concat(../@name,'_',@name)" />
<xsl:variable name="sqlParametersCount" select="count(parameters/parameter)" />
<xsl:variable name="inputSqlParameters" select="parameters/parameter[@direction='Input']" />
<xsl:variable name="inputSqlParametersCount" select="count(parameters/parameter[@direction='Input'])" />
<xsl:variable name="results" select="results/result[@isOutputParameter='false']" />
<xsl:variable name="resultsCount" select="count(results/result)" />
<xsl:variable name="resultType" select="results/result[1]/@type" />
<xsl:variable name="resultClass" select="concat(@name, 'Results')" />
<xsl:call-template name="ExecuteHeader" />
		{
			var cache = new global::DbSharper.Library.Caching.CachingService(<xsl:value-of select="$cacheKey" /><xsl:for-each select="$inputSqlParameters">, <xsl:value-of select="@camelCaseName" />
				<xsl:choose>
					<xsl:when test="@type='DateTime'">.ToShortDateString()</xsl:when>
					<xsl:when test="@type!='String'">.ToString()</xsl:when>
				</xsl:choose>
			</xsl:for-each>);

			var result = cache.Get() as <xsl:choose>
				<xsl:when test="$resultsCount=1"><xsl:value-of select="$resultType" /></xsl:when>
				<xsl:otherwise><xsl:value-of select="$resultClass" /></xsl:otherwise>
			</xsl:choose>;

			if (result == null)
			{<xsl:call-template name="ExecuteParameters" />
				result = new <xsl:choose><xsl:when test="$resultsCount=1"><xsl:value-of select="$resultType" /></xsl:when><xsl:when test="$resultsCount&gt;1"><xsl:value-of select="$resultClass" /></xsl:when></xsl:choose>();

				using (var reader = this.db.ExecuteReader(_dbCommand))
				{<xsl:for-each select="$results">
					<xsl:if test="position()!=1">

					reader.NextResult();
					</xsl:if>
					<xsl:choose>
					<xsl:when test="script:EndsWith(@type,'Model&gt;')">
					result<xsl:if test="$resultsCount&gt;1">.<xsl:value-of select="@name" /></xsl:if> = new <xsl:value-of select="@type" />();

					while (reader.Read())
					{
						result<xsl:if test="$resultsCount&gt;1">.<xsl:value-of select="@name" /></xsl:if>.Add(new <xsl:value-of select="script:GetModelType(@type)" />(reader));
					}</xsl:when>
					<xsl:otherwise>
					if(reader.Read())
					{
						result<xsl:if test="$resultsCount&gt;1">.<xsl:value-of select="@name" /></xsl:if> = new <xsl:value-of select="@type" />(reader);
					}
					else
					{
						result<xsl:if test="$resultsCount&gt;1">.<xsl:value-of select="@name" /></xsl:if> = null;
					}</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
				}
				<xsl:call-template name="ExecuteOutputParameters" />
				After_<xsl:value-of select="@name" />(<xsl:for-each select="$inputSqlParameters"><xsl:value-of select="@camelCaseName" /><xsl:if test="position()!=last()">, </xsl:if></xsl:for-each><xsl:if test="$inputSqlParametersCount&gt;0 and $resultsCount&gt;0">, </xsl:if>result);

				cache.Insert(result);
			}

			return result;
		}

		/// &lt;summary&gt;
		/// Remove cache of method <xsl:value-of select="@name" />.
		/// &lt;/summary&gt;
		internal void Remove_<xsl:value-of select="@name" />()
		{
			var cache = new global::DbSharper.Library.Caching.CachingService(<xsl:value-of select="$cacheKey" />);

			<xsl:choose>
				<xsl:when test="$inputSqlParametersCount=0">cache.Remove();</xsl:when>
				<xsl:otherwise>cache.RemoveBatch();</xsl:otherwise>
			</xsl:choose>
		}
		<xsl:for-each select="$inputSqlParameters">
		<xsl:variable name="cacheKeyPosition" select="position()" />
		/// &lt;summary&gt;
		/// Remove cache of method <xsl:value-of select="../../@name" />.
		/// &lt;/summary&gt;
		internal void Remove_<xsl:value-of select="../../@name" />(<xsl:for-each select="$inputSqlParameters">
				<xsl:if test="position()&lt;=$cacheKeyPosition">
					<xsl:value-of select="script:CSharpAlias(@type)" />
					<xsl:text> </xsl:text>
					<xsl:value-of select="@camelCaseName" />
					<xsl:if test="position()!=$cacheKeyPosition">, </xsl:if>
				</xsl:if>
			</xsl:for-each>)
		{
			var cache = new global::DbSharper.Library.Caching.CachingService(<xsl:value-of select="$cacheKey" /><xsl:for-each select="$inputSqlParameters">
				<xsl:if test="position()&lt;=$cacheKeyPosition">, <xsl:value-of select="@camelCaseName" />
				<xsl:choose>
					<xsl:when test="@type='DateTime'">.ToShortDateString()</xsl:when>
					<xsl:when test="@type!='String'">.ToString()</xsl:when>
				</xsl:choose>
				</xsl:if>
			</xsl:for-each>);

			<xsl:choose>
				<xsl:when test="$cacheKeyPosition=$inputSqlParametersCount">cache.Remove();</xsl:when>
				<xsl:otherwise>cache.RemoveBatch();</xsl:otherwise>
			</xsl:choose>
		}
		</xsl:for-each>
		#endregion</xsl:template>
</xsl:stylesheet>