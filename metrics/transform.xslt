<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml"/>
    <xsl:template match="/">
        <PostItems>
            <xsl:apply-templates/>
        </PostItems>
    </xsl:template>
    <xsl:template name="grid-item" match="//div[@class='grid-item']">
        <xsl:variable name="url" select="translate(./div[@class='post_container']/div[@class='post_footer']/a/@href, '>https://vk.com/wall', '')"/>
        <VkRepostViewModel>
            <Owner_Id>
                <xsl:value-of select="substring-before($url, '_')"/>
            </Owner_Id>
            <Id>
                <xsl:value-of select="substring-after($url, '_')"/>
            </Id>
        </VkRepostViewModel>
    </xsl:template>
</xsl:stylesheet>